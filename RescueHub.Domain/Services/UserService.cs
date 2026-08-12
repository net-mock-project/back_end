using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<User?> UpdateProfileAsync(
            Guid userId,
            string? fullName,
            string? phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            CancellationToken cancellationToken)
        { 
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                return null;
            }

            user.UpdateProfile(
                fullName,
                phone,
                dateOfBirth,
                gender);

            var isUpdated = await _userRepository.UpdateAsync(user, cancellationToken);

            if (!isUpdated)
                return null;

            return user;
        }
    }
}