using RescueHub.Domain.Entities;
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
            string? province)
        { 
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            user.UpdateProfile(
                fullName,
                phone,
                province);

            await _userRepository.UpdateAsync(user);

            return user;
        }
    }
}