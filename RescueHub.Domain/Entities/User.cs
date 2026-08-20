using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid RoleId { get; private set; }

        public string? RoleName { get; private set; }

        public GeoLocation? Location { get; private set; }

        public string? Province { get; private set; }

        public string? ProfileUrl { get; private set; }

        public string FullName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public string Phone { get; private set; } = null!;

        public DateOnly? DateOfBirth { get; private set; }

        public Gender? Gender { get; private set; }

        public string PasswordHash { get; private set; } = null!;

        public UserStatus Status { get; private set; } = UserStatus.Active;

        public bool IsVerified { get; private set; }

        public int ReliefRequestCount { get; private set; }

        public int DonationCount { get; private set; }

        public int TaskCompletedCount { get; private set; }

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();

        private User() { }

        // Dùng khi tạo mới User
        public User(
            Guid roleId,
            string? province,
            string fullName,
            string email,
            string phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            string passwordHash,
            bool isVerified)
            : base()
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException(
                    "Full name cannot be empty.",
                    nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email cannot be empty.",
                    nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException(
                    "Phone cannot be empty.",
                    nameof(phone));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(
                    "Password hash cannot be empty.",
                    nameof(passwordHash));

            RoleId = roleId;
            Province = province;
            FullName = fullName;
            Email = email;
            Phone = phone;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PasswordHash = passwordHash;

            Status = UserStatus.Active;
            IsVerified = isVerified;
        }

        // Dùng khi dựng lại User đã tồn từ database
        public User(
            Guid id,
            Guid roleId,
            GeoLocation? location,
            string? province,
            string? profileUrl,
            string fullName,
            string email,
            string phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            string passwordHash,
            UserStatus status,
            bool isVerified,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt,
            string? roleName = null,
            int reliefRequestCount = 0,
            int donationCount = 0,
            int taskCompletedCount = 0)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            RoleId = roleId;
            RoleName = roleName;
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
            ReliefRequestCount = reliefRequestCount;
            DonationCount = donationCount;
            TaskCompletedCount = taskCompletedCount;
        }

        // Thay đổi Role của User
        public void ChangeRole(Guid newRoleId)
        {
            if (newRoleId == Guid.Empty)
                throw new ArgumentException(
                    "Role ID cannot be empty.",
                    nameof(newRoleId));

            if (RoleId == newRoleId)
                return;

            RoleId = newRoleId;
            MarkUpdated();
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

        // Cập nhật avatar của user
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

        // Khóa tài khoản User
        public void LockAccount()
        {
            if (Status != UserStatus.Active)
                return;

            Status = UserStatus.Suspended;
            MarkUpdated();
        }

        // Mở khóa tài khoản User
        public void UnlockAccount()
        {
            if (Status != UserStatus.Suspended)
                return;

            Status = UserStatus.Active;
            MarkUpdated();
        }

        // Cập nhật vị trí hiện tại của User
        public void UpdateLocation(
            double latitude,
            double longitude)
        {
            if (
                Location != null &&
                Location.Latitude == latitude &&
                Location.Longitude == longitude)
            {
                return;
            }

            Location = new GeoLocation(
                latitude,
                longitude);

            MarkUpdated();
        }
    }
}