namespace RescueHub.API.Models
{
    // Dữ liệu Client gửi lên để cập nhật Profile
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Province { get; set; }
    }
}