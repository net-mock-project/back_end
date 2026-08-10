using System;
using NetTopologySuite.Geometries;

namespace RescueHub.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Requester"; // Mặc định là Requester
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";

        // --- CÁC CỜ TRẠNG THÁI (RẤT QUAN TRỌNG) ---
        public bool IsVerified { get; set; } = false;          // Đã xác thực OTP thành công chưa?

        // --- CÁC THÔNG TIN BỔ SUNG (Lần đăng nhập đầu tiên) ---
        public string? ProfileUrl { get; set; }
        public string? Province { get; set; } = string.Empty;
        public Point? Location { get; set; }

        // --- THỜI GIAN ---
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}