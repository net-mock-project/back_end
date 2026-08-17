using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Users
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }
    }
}