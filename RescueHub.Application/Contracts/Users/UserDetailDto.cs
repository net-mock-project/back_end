using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.Users
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string? ProfileUrl { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string? Province { get; set; }

        public UserStatus Status { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int ReliefRequestCount { get; set; }

        public int DonationCount { get; set; }

        public int TaskCompletedCount { get; set; }
    }
}