using NetTopologySuite.Geometries;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class ReliefTaskDataModel
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int RequiredVolunteers { get; set; }

        public int Priority { get; set; }

        public Point? Location { get; set; }

        public ReliefTaskStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ReliefRequestDataModel Request { get; set; } = null!;

        public ICollection<TaskSkillDataModel> TaskSkills { get; set; }
            = new List<TaskSkillDataModel>();

        public ICollection<TaskAssignmentDataModel> Assignments { get; set; }
            = new List<TaskAssignmentDataModel>();

        public ICollection<InventoryTransactionDataModel> InventoryTransactions { get; set; }
            = new List<InventoryTransactionDataModel>();
    }
}
