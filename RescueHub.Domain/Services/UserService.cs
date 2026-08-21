using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Common.Querying;

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
            return await _userRepository.GetProfileByIdAsync(
                userId,
                cancellationToken);
        }

        public async Task<PagedResult<User>> GetUsersAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            return await _userRepository.GetPagedAsync(
                criteria,
                cancellationToken);
        }

        public async Task<User?> GetUserDetailAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _userRepository.GetDetailByIdAsync(
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

        // Admin tạo User mới
        public async Task<User> CreateUserAsync(
            Guid roleId,
            string? province,
            string fullName,
            string email,
            string phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            string passwordHash,
            CancellationToken cancellationToken)
        {
            // Kiểm tra Role
            var roleExists = await _userRepository.RoleExistsAsync(
                roleId,
                cancellationToken);

            if (!roleExists)
            {
                throw new ArgumentException(
                    "Role does not exist.");
            }

            // Kiểm tra Email
            var emailExists = await _userRepository.EmailExistsAsync(
                email,
                cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "Email này đã được sử dụng.");
            }

            // Kiểm tra Phone
            var phoneExists = await _userRepository.PhoneExistsAsync(
                phone,
                cancellationToken);

            if (phoneExists)
            {
                throw new InvalidOperationException(
                    "Số điện thoại này đã được sử dụng.");
            }

            // Admin tạo trực tiếp nên User được xác thực
            var user = new User(
                roleId,
                province,
                fullName,
                email,
                phone,
                dateOfBirth,
                gender,
                passwordHash,
                true);

            await _userRepository.AddAsync(
                user,
                cancellationToken);

            return user;
        }

        // Admin khóa tài khoản User
        public async Task<User?> LockUserAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user == null || user.DeletedAt.HasValue)
            {
                return null;
            }

            user.LockAccount();

            var isUpdated =
                await _userRepository.UpdateStatusAsync(
                    user,
                    cancellationToken);

            return isUpdated ? user : null;
        }

        // Admin mở khóa tài khoản User
        public async Task<User?> UnlockUserAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user == null || user.DeletedAt.HasValue)
            {
                return null;
            }

            user.UnlockAccount();

            var isUpdated =
                await _userRepository.UpdateStatusAsync(
                    user,
                    cancellationToken);

            return isUpdated ? user : null;
        }


        public async Task<User?> UpdateLocationAsync(
            Guid userId,
            double latitude,
            double longitude,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user == null ||
                user.DeletedAt.HasValue)
            {
                return null;
            }

            user.UpdateLocation(
                latitude,
                longitude);

            var isUpdated =
                await _userRepository.UpdateLocationAsync(
                    user,
                    cancellationToken);

            return isUpdated
                ? user
                : null;
        }

        public async Task<List<User>> GetUsersWithinRangeAsync(
            double latitude,
            double longitude,
            double radius,
            CancellationToken cancellationToken)
        {
            return await _userRepository.GetUsersWithinRangeAsync(
                latitude,
                longitude,
                radius,
                cancellationToken);
        }
    }
}