using NetTopologySuite.Geometries;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class UserDataModel
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Gender? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Point? Location { get; set; }

        public string? Province { get; set; }

        public string? ProfileUrl { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public UserStatus Status { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public RoleDataModel Role { get; set; } = null!;

        public VolunteerDataModel? Volunteer { get; set; }

        public ICollection<ReliefRequestDataModel> ReliefRequests { get; set; }
            = new List<ReliefRequestDataModel>();

        public ICollection<ReliefRequestDataModel> CoordinatedReliefRequests { get; set; }
            = new List<ReliefRequestDataModel>();

        public ICollection<TaskAssignmentDataModel> AssignedTasks { get; set; }
            = new List<TaskAssignmentDataModel>();

        public ICollection<DonationDataModel> Donations { get; set; }
            = new List<DonationDataModel>();

        public ICollection<NotificationDataModel> Notifications { get; set; }
            = new List<NotificationDataModel>();

        public ICollection<AuditLogDataModel> AuditLogs { get; set; }
            = new List<AuditLogDataModel>();
    }
}