using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Users
{
    public class CreateUserRequest
    {
        public string RoleName { get; set; } = null!;

        public string? Province { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string Password { get; set; } = null!;
    }
}