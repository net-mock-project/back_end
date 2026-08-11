namespace RescueHub.Application.Contracts
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Province { get; set; }

        public string? ProfileUrl { get; set; }
    }
}