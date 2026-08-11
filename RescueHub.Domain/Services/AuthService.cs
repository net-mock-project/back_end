using RescueHub.Domain.Entities.RegisterDTOs;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using BCrypt.Net;
using RescueHub.Infrastructure.SqlServer.Services;

namespace RescueHub.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IEmailService emailService)
        {
            _authRepository = authRepository;
            _emailService = emailService;
        }

        // --- BƯỚC 1: GỬI MÃ OTP QUA EMAIL ---
        public async Task<bool> SendOtpAsync(string email)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(email);
            if (existingUser != null && existingUser.IsVerified)
                return false;

            string otpCode = new Random().Next(100000, 999999).ToString();

            Console.WriteLine($"\n========================================");
            Console.WriteLine($"[TESTING] Ma OTP cho email {email} la: {otpCode}");
            Console.WriteLine($"========================================\n");

            await _emailService.SendEmailAsync(email, "Mã OTP xác thực tài khoản", $"Mã OTP của bạn là: {otpCode}. Mã này sẽ hết hạn sau 5 phút.");

            var otpEntry = new OtpVerification
            {
                Email = email,
                Code = otpCode,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _authRepository.AddOtpAsync(otpEntry);
            await _authRepository.SaveChangesAsync();

            return true;
        }

        // --- GỬI LẠI OTP NẾU NGƯỜI DÙNG YÊU CẦU ---
        public async Task<bool> ResendOtpAsync (string email)
        {
            await _authRepository.RemoveOldOtpAsync(email);
            return await SendOtpAsync(email);
        }

        // --- BƯỚC 2: XÁC NHẬN OTP VÀ HOÀN TẤT ĐĂNG KÝ ---
        public async Task<string> VerifyAndRegisterAsync(RegisterDto dto)
        {
            

            // 1. Lấy ra mã OTP mới nhất của EMAIL này từ Database
            var otpRecord = await _authRepository.GetLatestOtpByEmailAsync(dto.Email);

            // === DEBUG NÀY ĐỂ KIỂM TRA ===
            if (otpRecord == null)
            {
                Console.WriteLine("[DEBUG] Khong tim thay ma OTP nao trong DB cho email nay!");
            }
            else
            {
                Console.WriteLine($"[DEBUG] Tim thay OTP trong DB: Code DB = [{otpRecord.Code}], Code User nhap = [{dto.OtpCode}]");
                Console.WriteLine($"[DEBUG] Thoi gian het han: [{otpRecord.ExpiredAt}], Thoi gian hien tai (UTC): [{DateTime.UtcNow}]");
            }
            // ==========================================

            // 2. Kiểm tra xem có OTP không, mã có khớp không và còn hạn không
            if (otpRecord == null || otpRecord.Code != dto.OtpCode || otpRecord.ExpiredAt < DateTime.UtcNow)
            {
                return "InvalidOtp";
            }

            // 3. Tiến hành tạo User chính thức
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                PasswordHash = passwordHash,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.AddUserAsync(newUser);

            // 4. Xóa mã OTP đi vì đã dùng rồi
            await _authRepository.RemoveOtpAsync(otpRecord);

            await _authRepository.SaveChangesAsync();
            return "Success";
        }
    }
}