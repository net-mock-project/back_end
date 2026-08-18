using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task ValidateNewUserRegistrationAsync(string email, CancellationToken cancellationToken)
        {
            var existingUser = await _authRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null && existingUser.IsVerified)
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }
        }
       
        public async Task<bool> RegisterAsync(
            string address,
            string fullName,
            string email,
            String phoneNumber,
            DateTime dateOfBirth,
            Gender gender,
            string passwordHash,
            CancellationToken cancellationToken)
        {
            var roleId = await _authRepository.GetRoleIdAsync("Requester", cancellationToken);

            if (roleId == null)
            {
                throw new InvalidOperationException("Không tìm thấy Id của Requester");
            }

            var newUser = new User(
                Guid.NewGuid(),
                roleId.Value,
                null,
                address,
                null,
                fullName,
                email,
                phoneNumber,
                DateOnly.FromDateTime(dateOfBirth),
                gender,
                passwordHash,
                UserStatus.Active,
                true,
                DateTime.UtcNow,
                null,
                null
            );

            await _authRepository.AddAsync(newUser, cancellationToken);

            return true;
        }

        public async Task<(User?, string?)> LoginAsync(
            string email,
            CancellationToken cancellationToken)
        {
            var user = await _authRepository.GetByEmailAsync(email, cancellationToken);

            if (user is null)
                return (null, null);

            if (user.DeletedAt.HasValue)
                return (null, null);

            if (!user.IsVerified)
                return (null, null);

            if (user.Status != UserStatus.Active)
                return (null, null);

            var roleName = await _authRepository.GetRoleNameAsync(
                user.RoleId,
                cancellationToken);

            if (roleName is null)
                return (null, null);

            return (user, roleName);
        }
    }
}