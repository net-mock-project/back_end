using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class WarehouseTransaction
    {
        public Guid Id { get; set; }
        public Guid WarehouseInventoryId { get; set; }
        public int Quantity { get; set; }
        public WarehouseTransactionType TransactionType { get; set; }
        public WarehouseTransactionStatus Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public WarehouseInventory WarehouseInventories { get; set; } = null!;
        public User Creator { get; set; } = null!;
        public DonationTransaction? DonationTransactions { get; set; }
    }
}