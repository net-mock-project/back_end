using RescueHub.Domain.Enums;

namespace RescueHub.API.Models
{
    // Dữ liệu trả về sau khi cập nhật Profile
    public class UserProfileResponse
    {
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
    }
}