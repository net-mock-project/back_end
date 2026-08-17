namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class SupplyDataModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Category { get; set; }

        public string Unit { get; set; } = null!;

        public int MinimumStock { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<WarehouseInventoryDataModel> WarehouseInventories { get; set; }
            = new List<WarehouseInventoryDataModel>();
    }
}
