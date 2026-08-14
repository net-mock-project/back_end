using NetTopologySuite.Geometries;
using RescueHub.Domain.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    // Data Model ánh xạ tới bảng Users trong SQL Server
    public class UserDataModel
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        // Location lưu kiểu geography trong SQL Server
        public Point? Location { get; set; }

        public string? Province { get; set; }

        public string? ProfileUrl { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string PasswordHash { get; set; } = null!;

        public string Status { get; set; } = null!;

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeleteAt { get; set; }

        public RoleDataModel? Role {  get; set; }
    }
}