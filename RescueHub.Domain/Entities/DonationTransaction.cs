using RescueHub.Domain.Common;

namespace RescueHub.Domain.Entities
{
    public class DonationTransaction
    {
        public Guid TransactionId { get; set; }
        public Guid DonationId { get; set; }

        public WarehouseTransaction WarehouseTransactions { get; set; } = null!;
        public Donation Donations { get; set; } = null!;

        private DonationTransaction() { }

        public DonationTransaction(Guid donationId, Guid transactionId)
        {
            DonationId = donationId;
            TransactionId = transactionId;
        }
    }
}