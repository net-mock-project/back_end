using RescueHub.Domain.Entities.RegisterDTOs;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using RescueHub.Infrastructure.SqlServer.Persistence;
using BCrypt.Net;

namespace RescueHub.Infrastructure.SqlServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- BƯỚC 1: GỬI MÃ OTP ---
        public async Task<bool> SendOtpAsync(string phoneNumber)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (existingUser != null && existingUser.IsVerified)
                return false;

            string otpCode = new Random().Next(100000, 999999).ToString();

            Console.WriteLine($"\n========================================");
            Console.WriteLine($"[TESTING] Ma OTP cho so {phoneNumber} la: {otpCode}");
            Console.WriteLine($"========================================\n");

            var otpEntry = new OtpVerification
            {
                PhoneNumber = phoneNumber,
                Code = otpCode,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5)
            };
            _context.OtpVerifications.Add(otpEntry);
            await _context.SaveChangesAsync();

            return true;
        }

        
        // --- BƯỚC 2: XÁC NHẬN OTP VÀ HOÀN TẤT ĐĂNG KÝ ---
        public async Task<string> VerifyAndRegisterAsync(RegisterDto dto)
        {
            // 1. Kiểm tra xem Email hoặc Số điện thoại đã tồn tại tài khoản chính thức chưa
            var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email || u.PhoneNumber == dto.PhoneNumber);
            if (userExists)
            {
                return "UserExists";
            }

            // 2. Lấy ra mã OTP mới nhất của số điện thoại này từ Database
            var otpRecord = await _context.OtpVerifications
                .Where(o => o.PhoneNumber == dto.PhoneNumber)
                .OrderByDescending(o => o.ExpiredAt)
                .FirstOrDefaultAsync();

            // === DEBUG NÀY ĐỂ KIỂM TRA ===
            if (otpRecord == null)
            {
                Console.WriteLine("[DEBUG] Khong tim thay ma OTP nao trong DB cho so nay!");
            }
            else
            {
                Console.WriteLine($"[DEBUG] Tim thay OTP trong DB: Code DB = [{otpRecord.Code}], Code User nhap = [{dto.OtpCode}]");
                Console.WriteLine($"[DEBUG] Thoi gian het han: [{otpRecord.ExpiredAt}], Thoi gian hien tai (UTC): [{DateTime.UtcNow}]");
            }
            // ==========================================

            // 3. Kiểm tra xem có OTP không, mã có khớp không và còn hạn không
            if (otpRecord == null || otpRecord.Code != dto.OtpCode || otpRecord.ExpiredAt < DateTime.UtcNow)
            {
                return "InvalidOtp";
            }

            // 4. Tiến hành tạo User chính thức
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

            _context.Users.Add(newUser);

            // 5. Xóa mã OTP đi vì đã dùng rồi
            _context.OtpVerifications.Remove(otpRecord);

            await _context.SaveChangesAsync();
            return "Success";
        }
    }
}