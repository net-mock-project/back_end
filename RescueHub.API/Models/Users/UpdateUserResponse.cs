using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Users
{
    public class UpdateUserResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}