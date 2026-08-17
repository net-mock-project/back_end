using NetTopologySuite.Geometries;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class WarehouseDataModel
    {
        public Guid Id { get; set; }

        public Point Location { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? ManagerName { get; set; }

        public string Province { get; set; } = null!;

        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ICollection<WarehouseInventoryDataModel> Inventories { get; set; }
            = new List<WarehouseInventoryDataModel>();
    }
}
