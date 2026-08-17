namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class WarehouseInventoryDataModel
    {
        public Guid Id { get; set; }

        public Guid WarehouseId { get; set; }

        public Guid SupplyId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public WarehouseDataModel Warehouse { get; set; } = null!;

        public SupplyDataModel Supply { get; set; } = null!;

        public ICollection<WarehouseTransactionDataModel> Transactions { get; set; }
            = new List<WarehouseTransactionDataModel>();
    }
}
