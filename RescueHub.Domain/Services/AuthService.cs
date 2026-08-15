using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICacheService _cache;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        private static readonly Guid DefaultUserRoleId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");

        public AuthService(
            ICacheService cache,
            IAuthRepository authRepository,
            IEmailService emailService,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _cache = cache;
            _authRepository = authRepository;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
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
            // Kiểm tra Email
            var existingUser =
                await _authRepository.GetByEmailAsync(email);

            if (existingUser != null && existingUser.IsVerified)
            {
                throw new InvalidOperationException(
                    "Email này đã được sử dụng.");
            }

            // Kiểm tra số điện thoại
            var existingPhone =
                await _authRepository.GetByPhoneAsync(phoneNumber);

            if (existingPhone != null && existingPhone.IsVerified)
            {
                throw new InvalidOperationException(
                    "Số điện thoại này đã được sử dụng.");
            }

            var otpCode = new Random().Next(100000, 999999).ToString();

            Console.WriteLine($"\n==========================================");
            Console.WriteLine($"[DEV OTP] Email: {email} | Mã OTP: {otpCode}");
            Console.WriteLine($"==========================================\n");

            var passwordHash = _passwordHasher.Hash(password);

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

            await _cache.SetAsync(GetCacheKey(email), pendingData, TimeSpan.FromMinutes(5));

            await _emailService.SendEmailAsync(email, "Mã xác thực đăng ký RescueHub", $"Mã OTP của bạn là: {otpCode}. Có hiệu lực trong 5 phút.");

            return true;
        }

        public async Task<bool> ResendOtpAsync(string email)
        {
            var pendingData =
                await _cache.GetAsync<PendingRegistration>(
                    GetCacheKey(email));

            if (pendingData == null)
            {
                throw new InvalidOperationException(
                    "Yêu cầu đăng ký không tồn tại hoặc đã hết hạn. Vui lòng đăng ký lại.");
            }
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

            await _cache.SetAsync(
                GetCacheKey(email),
                updatedData,
                TimeSpan.FromMinutes(5));

            await _emailService.SendEmailAsync(email, "Mã xác thực OTP mới", $"Mã OTP mới của bạn là: {newOtpCode}.");

            return true;
        }

        public async Task<bool> RegisterAsync(string email, string otpCode)
        {
            var pendingData =
                await _cache.GetAsync<PendingRegistration>(
                    GetCacheKey(email));

            if (pendingData == null)
            {
                throw new InvalidOperationException(
                    "Mã OTP đã hết hạn hoặc không tồn tại.");
            }

            if (pendingData.OtpCode != otpCode)
            {
                throw new ArgumentException(
                    "Mã OTP không chính xác.");
            }

            if (DateTime.UtcNow > pendingData.ExpiresAt)
            {
                throw new InvalidOperationException("Mã OTP đã hết hạn.");
            }

            var newUser = new User(
                Guid.NewGuid(),
                DefaultUserRoleId,
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

        public async Task<(string?, User?)> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            // 1. Tìm User theo Email
            var user =
                await _authRepository.GetByEmailAsync(email);

            if (user is null)
            {
                return (null, null);
            }

            // 2. Kiểm tra tài khoản đã bị xóa
            if (user.DeleteAt.HasValue)
            {
                return (null, null);
            }

            // 3. Kiểm tra tài khoản đã verify
            if (!user.IsVerified)
            {
                return (null, null);
            }

            // 4. Kiểm tra trạng thái tài khoản
            if (!string.Equals(
                    user.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                return (null, null);
            }

            // 5. Kiểm tra password
            var isPasswordValid =
                _passwordHasher.Verify(
                    password,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                return (null, null);
            }

            // 6. Tạo JWT
            var token =
                _jwtService.GenerateToken(
                    user.Id,
                    user.Email,
                    user.RoleId);

            // 7. Trả kết quả login
            return (token, user);
        }

        // ============================================================
        // CACHE KEY
        // ============================================================

        private static string GetCacheKey(string email)
        {
            return $"auth:pending-reg:{email.ToLower()}";
        }
    }
}