using System.ComponentModel.DataAnnotations;

namespace RescueHub.Domain.Entities.RegisterDTOs
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}