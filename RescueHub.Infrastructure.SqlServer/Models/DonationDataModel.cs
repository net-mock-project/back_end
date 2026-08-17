using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class DonationDataModel
    {
        public Guid Id { get; set; }

        public Guid DonatorId { get; set; }

        public DonationStatus Status { get; set; }

        public DateTime DonationDate { get; set; }

        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? Remark { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public UserDataModel Donator { get; set; } = null!;

        public UserDataModel? Approver { get; set; }

        public ICollection<DonationTransactionDataModel> Transactions { get; set; }
            = new List<DonationTransactionDataModel>();
    }
}
