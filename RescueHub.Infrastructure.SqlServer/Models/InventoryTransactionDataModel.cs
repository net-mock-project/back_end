namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class InventoryTransactionDataModel
    {
        public Guid TransactionId { get; set; }

        public Guid TaskId { get; set; }

        public WarehouseTransactionDataModel Transaction { get; set; }
            = null!;

        public ReliefTaskDataModel Task { get; set; }
            = null!;
    }
}
