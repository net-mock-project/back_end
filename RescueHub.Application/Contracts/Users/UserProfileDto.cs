namespace RescueHub.Application.Contracts.Users
{
    public class UserProfileDto
    {
        // ID của User
        public int UserId { get; set; }

        // Họ và tên
        public string FullName { get; set; } = string.Empty;

        // Email
        public string Email { get; set; } = string.Empty;

        // Số điện thoại
        public string? Phone { get; set; }

        // Tỉnh / thành phố
        public string? Province { get; set; }

        // Thời gian cập nhật gần nhất
        public DateTime? UpdatedAt { get; set; }
    }
}