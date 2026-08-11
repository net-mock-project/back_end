using RescueHub.Domain.Common;

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
            PasswordHash = passwordHash;
            Status = status;
            IsVerified = isVerified;
            DeleteAt = deleteAt;
        }


        // Chỉ cập nhật khi dữ liệu thực sự thay đổi
        public void UpdateProfile(
            string? fullName,
            string? phone,
            string? province)
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

            if (province != null && province != Province)
            {
                Province = province;
                isChanged = true;
            }

            if (isChanged)
                MarkUpdated();
        }
    }
}