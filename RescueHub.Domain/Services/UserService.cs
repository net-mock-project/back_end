using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        // Inject Repository để Service thao tác với dữ liệu User
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> UpdateProfileAsync(
            int userId,
            string? fullName,
            string? email,
            string? phone,
            string? province,
            CancellationToken cancellationToken = default)
        {
            // 1. Tìm User hiện tại
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // 2. PATCH phải có ít nhất một trường cần cập nhật
            if (fullName is null
                && email is null
                && phone is null
                && province is null)
            {
                throw new ArgumentException(
                    "At least one field must be provided.");
            }

            // 3. Cập nhật FullName
            if (fullName is not null)
            {
                var newFullName = fullName.Trim();

                if (string.IsNullOrWhiteSpace(newFullName))
                {
                    throw new ArgumentException(
                        "Full name cannot be empty.");
                }

                user.FullName = newFullName;
            }

            // 4. Cập nhật Email
            if (email is not null)
            {
                var newEmail = email.Trim();

                if (string.IsNullOrWhiteSpace(newEmail))
                {
                    throw new ArgumentException(
                        "Email cannot be empty.");
                }

                // Chỉ kiểm tra trùng nếu Email thực sự thay đổi
                if (!string.Equals(
                        newEmail,
                        user.Email,
                        StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists =
                        await _userRepository.EmailExistsAsync(
                            newEmail,
                            userId,
                            cancellationToken);

                    if (emailExists)
                    {
                        throw new ArgumentException(
                            "Email này đã được sử dụng.");
                    }

                    user.Email = newEmail;
                }
            }

            // 5. Cập nhật Phone
            if (phone is not null)
            {
                var newPhone = phone.Trim();

                if (string.IsNullOrWhiteSpace(newPhone))
                {
                    throw new ArgumentException(
                        "Phone cannot be empty.");
                }

                user.Phone = newPhone;
            }

            // 6. Cập nhật Province
            if (province is not null)
            {
                var newProvince = province.Trim();

                if (string.IsNullOrWhiteSpace(newProvince))
                {
                    throw new ArgumentException(
                        "Province cannot be empty.");
                }

                user.Province = newProvince;
            }

            // 7. Ghi thời gian cập nhật
            user.UpdatedAt = DateTime.UtcNow;

            // 8. Lưu thay đổi xuống Database
            await _userRepository.SaveChangesAsync(
                cancellationToken);

            // 9. Trả User sau khi cập nhật
            return user;
        }
    }
}