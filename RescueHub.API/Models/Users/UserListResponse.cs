using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Users
{
    public class UserListResponse
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string? Province { get; set; }

        public string? ProfileUrl { get; set; }

        public UserStatus Status { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}