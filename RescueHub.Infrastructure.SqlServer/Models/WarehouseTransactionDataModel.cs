using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class WarehouseTransactionDataModel
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

        public WarehouseInventoryDataModel WarehouseInventory { get; set; }
            = null!;

        public UserDataModel Creator { get; set; }
            = null!;

        public InventoryTransactionDataModel? InventoryTransaction { get; set; }

        public DonationTransactionDataModel? DonationTransaction { get; set; }
    }
}
