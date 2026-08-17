using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.Users
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? Province { get; set; }
        public string? ProfileUrl { get; set; }
    }
}