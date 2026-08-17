using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Users
{
    // Dữ liệu Profile trả về Client
    public class GetProfileResponse
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? Province { get; set; }
        public string? ProfileUrl { get; set; }
    }
}