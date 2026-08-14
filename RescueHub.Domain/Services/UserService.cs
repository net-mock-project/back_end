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

        public async Task<User?> GetProfileAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);
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


        public async Task<User?> UpdateAvatarAsync(
            Guid userId,
            string profileUrl,
            CancellationToken cancellationToken)
        {
            // Lấy user cần cập nhật
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user == null)
                return null;

            // Cập nhật URL avatar trong Domain
            user.UpdateAvatar(profileUrl);

            // Gửi User đã thay đổi xuống Repository
            var isUpdated = await _userRepository.UpdateAvatarAsync(
                user,
                cancellationToken);

            return isUpdated ? user : null;
        }
    }
}