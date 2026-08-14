using RescueHub.Domain.Common;
using RescueHub.Domain.Enums;

namespace RescueHub.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid RoleId { get; private set; }

        public GeoLocation? Location { get; private set; }

        public string? Province { get; private set; }

        public string? ProfileUrl { get; private set; }

        public string FullName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public string? Phone { get; private set; }

        public DateOnly? DateOfBirth { get; private set; }

        public Gender? Gender { get; private set; }

        public string PasswordHash { get; private set; } = null!;

        public string Status { get; private set; } = null!;

        public bool IsVerified { get; private set; }

        public DateTime? DeleteAt { get; private set; }


        private User() { }


        // Dùng khi dựng lại User đã tồn tại từ database
        public User(
            Guid id,
            Guid roleId,
            GeoLocation? location,
            string? province,
            string? profileUrl,
            string fullName,
            string email,
            string? phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            string passwordHash,
            string status,
            bool isVerified,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deleteAt)
            : base(id, createdAt, updatedAt)
        {
            RoleId = roleId;
            Location = location;
            Province = province;
            ProfileUrl = profileUrl;
            FullName = fullName;
            Email = email;
            Phone = phone;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PasswordHash = passwordHash;
            Status = status;
            IsVerified = isVerified;
            DeleteAt = deleteAt;
        }


        // Chỉ cập nhật khi dữ liệu thực sự thay đổi
        public void UpdateProfile(
            string? fullName,
            string? phone,
            DateOnly? dateOfBirth,
            Gender? gender)
        {
            var isChanged = false;

            if (fullName != null && fullName != FullName)
            {
                if (string.IsNullOrWhiteSpace(fullName))
                    throw new ArgumentException(
                        "Full name cannot be empty.",
                        nameof(fullName));

                FullName = fullName;
                isChanged = true;
            }

            if (phone != null && phone != Phone)
            {
                Phone = phone;
                isChanged = true;
            }

            if (dateOfBirth != null && dateOfBirth != DateOfBirth)
            {
                DateOfBirth = dateOfBirth;
                isChanged = true;
            }

            if (gender != null && gender != Gender)
            {
                Gender = gender;
                isChanged = true;
            }

            if (isChanged)
                MarkUpdated();
        }

        // Cập nhật avartar của user
        public void UpdateAvatar(string profileUrl)
        {
            if (string.IsNullOrWhiteSpace(profileUrl))
                throw new ArgumentException(
                    "Profile URL cannot be empty.",
                    nameof(profileUrl));

            if (ProfileUrl == profileUrl)
                return;

            ProfileUrl = profileUrl;
            MarkUpdated();
        }
    }
}