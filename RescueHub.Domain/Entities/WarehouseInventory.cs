using RescueHub.Domain.Common;

namespace RescueHub.Domain.Entities
{
    public class WarehouseInventory : BaseEntity
    {
        public Guid WarehouseId { get; set; }
        public Guid SupplyId { get; set; }
        public int Quantity { get; set; }

        public Warehouse Warehouses { get; set; } = null!;
        public Supply Supplys { get; set; } = null!;
        public ICollection<WarehouseTransaction> WarehouseTransactions { get; set; } = new List<WarehouseTransaction>();

        private WarehouseInventory() { }

        public WarehouseInventory(
            Guid id,
            Guid warehouseId,
            Guid supplyId,
            int quantity,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            WarehouseId = warehouseId;
            SupplyId = supplyId;
            Quantity = quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            Quantity = newQuantity;
            MarkUpdated(); 
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
            MarkUpdated(); 
        }
    }
}