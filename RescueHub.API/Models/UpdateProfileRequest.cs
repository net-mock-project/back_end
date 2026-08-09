using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models.Users
{
    public class UpdateProfileRequest
    {
        // Họ và tên mới
        public string? FullName { get; set; }

        // Email mới
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        // Số điện thoại mới
        public string? Phone { get; set; }

        // Tỉnh / thành phố mới
        public string? Province { get; set; }
    }
}