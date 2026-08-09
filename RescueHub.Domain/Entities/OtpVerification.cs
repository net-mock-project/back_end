using System;

namespace RescueHub.Domain.Entities
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty; // Số điện thoại nhận mã
        public string Code { get; set; } = string.Empty;        // Mã OTP (ví dụ: "686868")
        public DateTime ExpiredAt { get; set; }                 // Thời gian hết hạn (ví dụ: sau 5 phút)
    }
}