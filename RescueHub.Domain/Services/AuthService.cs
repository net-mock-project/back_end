using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Services
{
    public class AuthService : IAuthService
    {
        //private readonly ICacheService _cache;
        private readonly IAuthRepository _authRepository;
        //private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            //ICacheService cache,
            IAuthRepository authRepository,
            //IEmailService emailService,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            //_cache = cache;
            _authRepository = authRepository;
            //_emailService = emailService;
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

        // ============================================================
        // LOGIN
        // ============================================================

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