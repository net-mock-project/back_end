using NetTopologySuite.Geometries;

namespace RescueHub.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public Point? Location { get; set; }

        public string? Province { get; set; }

        public string? ProfileUrl { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeleteAt { get; set; }
    }
}