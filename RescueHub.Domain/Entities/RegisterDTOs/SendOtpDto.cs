using System.ComponentModel.DataAnnotations;

namespace RescueHub.Domain.Entities.RegisterDTOs
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; } = string.Empty;
    }
}