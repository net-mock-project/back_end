using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class Donation : BaseEntity
    {
        public Guid DonatorId { get; set; }
        public DonationStatus Status { get; set; }
        public DateTime DonationDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Remark { get; set; }

        public User Donator { get; set; } = null!;
        public User? Approver { get; set; }
        public ICollection<DonationTransaction> DonationTransactions { get; set; } = new List<DonationTransaction>();

        private Donation() { }

        public Donation(
            Guid id,
            Guid donatorId,
            DonationStatus status,
            DateTime donationDate,
            Guid? approvedBy,
            DateTime? approvedAt,
            string? remark,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt = null)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            DonatorId = donatorId;
            Status = status;
            DonationDate = donationDate;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
            Remark = remark;
        }

        public void UpdateStatus(DonationStatus newStatus, Guid coordinatorId)
        {
            Status = newStatus;
            ApprovedBy = coordinatorId;
            ApprovedAt = DateTime.UtcNow;
            MarkUpdated();
        }

        public void Cancel()
        {
            Status = DonationStatus.Cancelled;
            MarkUpdated(); 
        }

        public void Complete(Guid coordinatorId)
        {
            Status = DonationStatus.Completed;
            ApprovedBy = coordinatorId;
            ApprovedAt = DateTime.UtcNow;
            MarkUpdated(); 
        }

        public void Reject(Guid coordinatorId)
        {
            Status = DonationStatus.Rejected;
            ApprovedBy = coordinatorId;
            ApprovedAt = DateTime.UtcNow;
            MarkUpdated(); 
        }

        public void UpdateDate(DateTime donationDate)
        {
            DonationDate = donationDate;
            MarkUpdated(); 
        }
    }
}