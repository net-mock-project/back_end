using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models
{
    public class RegisterRequest
    {
        
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mã OTP phải từ 6 ký tự trở lên")]
        public required string OtpCode { get; set; }
    }
}