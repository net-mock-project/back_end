using Microsoft.Extensions.Caching.Distributed;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums; // Đảm bảo đã có using này để nhận diện Gender
using RescueHub.Domain.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RescueHub.Domain.Services // Hoặc namespace hiện tại của bạn trong Domain
{
    public class AuthService : IAuthService
    {
        private readonly IDistributedCache _cache;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;

        public AuthService(
            IDistributedCache cache,
            IAuthRepository authRepository,
            IEmailService emailService)
        {
            _cache = cache;
            _authRepository = authRepository;
            _emailService = emailService;
        }

        private record PendingRegistration(
            string FullName,
            DateTime DateOfBirth,
            string Email,
            string PhoneNumber,
            Gender Gender,
            string PasswordHash,
            string Address,
            string OtpCode,
            DateTime ExpiresAt
        );

        public async Task<bool> SendOtpAsync(
            string fullName,
            DateTime dateOfBirth,
            string email,
            string phoneNumber,
            Gender gender,
            string password,
            string address)
        {
            var existingUser = await _authRepository.GetByEmailAsync(email);
            if (existingUser != null && existingUser.IsVerified)
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            var otpCode = new Random().Next(100000, 999999).ToString();

            Console.WriteLine($"\n==========================================");
            Console.WriteLine($"[DEV OTP] Email: {email} | Mã OTP: {otpCode}");
            Console.WriteLine($"==========================================\n");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var pendingData = new PendingRegistration(
                fullName,
                dateOfBirth,
                email,
                phoneNumber,
                gender,
                passwordHash,
                address,
                otpCode,
                DateTime.UtcNow.AddMinutes(5)
            );

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(
                GetCacheKey(email),
                JsonSerializer.Serialize(pendingData),
                cacheOptions
            );

            await _emailService.SendEmailAsync(email, "Mã xác thực đăng ký RescueHub", $"Mã OTP của bạn là: {otpCode}. Có hiệu lực trong 5 phút.");

            return true;
        }

        public async Task<bool> ResendOtpAsync(string email)
        {
            var cachedData = await _cache.GetStringAsync(GetCacheKey(email));
            if (string.IsNullOrEmpty(cachedData))
            {
                throw new InvalidOperationException("Yêu cầu đăng ký không tồn tại hoặc đã hết hạn. Vui lòng đăng ký lại.");
            }

            var pendingData = JsonSerializer.Deserialize<PendingRegistration>(cachedData);
            if (pendingData == null) return false;

            var newOtpCode = new Random().Next(100000, 999999).ToString();

            Console.WriteLine($"\n==========================================");
            Console.WriteLine($"[DEV RESEND OTP] Email: {email} | Mã OTP Mới: {newOtpCode}");
            Console.WriteLine($"==========================================\n");

            var updatedData = pendingData with
            {
                OtpCode = newOtpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(
                GetCacheKey(email),
                JsonSerializer.Serialize(updatedData),
                cacheOptions
            );

            await _emailService.SendEmailAsync(email, "Mã xác thực OTP mới", $"Mã OTP mới của bạn là: {newOtpCode}.");

            return true;
        }

        public async Task<bool> RegisterAsync(string email, string otpCode)
        {
            var cachedData = await _cache.GetStringAsync(GetCacheKey(email));
            if (string.IsNullOrEmpty(cachedData))
            {
                throw new InvalidOperationException("Mã OTP đã hết hạn hoặc không tồn tại.");
            }

            var pendingData = JsonSerializer.Deserialize<PendingRegistration>(cachedData);
            if (pendingData == null || pendingData.OtpCode != otpCode)
            {
                throw new ArgumentException("Mã OTP không chính xác.");
            }

            if (DateTime.UtcNow > pendingData.ExpiresAt)
            {
                throw new InvalidOperationException("Mã OTP đã hết hạn.");
            }

            var newUser = new User(
                Guid.NewGuid(),
                Guid.Empty,
                null,
                pendingData.Address,
                null,
                pendingData.FullName,
                pendingData.Email,
                pendingData.PhoneNumber,
                DateOnly.FromDateTime(pendingData.DateOfBirth),
                pendingData.Gender,
                pendingData.PasswordHash,
                "Active",
                true,
                DateTime.UtcNow,
                null,
                null
            );

            await _authRepository.AddAsync(newUser);
            await _cache.RemoveAsync(GetCacheKey(email));

            return true;
        }

        private static string GetCacheKey(string email) => $"auth:pending-reg:{email.ToLower()}";
    }
}