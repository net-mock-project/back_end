
namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class DonationTransactionDataModel
    {
        public Guid TransactionId { get; set; }

        public Guid DonationId { get; set; }

        public WarehouseTransactionDataModel Transaction { get; set; }
            = null!;

        public DonationDataModel Donation { get; set; }
            = null!;
    }
}
