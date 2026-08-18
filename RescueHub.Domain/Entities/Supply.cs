using RescueHub.Domain.Common;

namespace RescueHub.Domain.Entities
{
    public class Supply : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string Unit { get; set; } = null!;
        public int MinimumStock { get; set; }

        public ICollection<WarehouseInventory> WarehouseInventories { get; set; } = new List<WarehouseInventory>();

        private Supply() { }

        public Supply(
            Guid id,
            string name,
            string category,
            string unit,
            int minimumStock,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            Name = name;
            Category = category;
            Unit = unit;
            MinimumStock = minimumStock;
        }

    }
}