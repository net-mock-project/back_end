using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class Warehouse : BaseEntity
    {    
        public string Name { get; set; } = null!;     
        public string Province { get; set; } = null!;
        public GeoLocation Location { get; set; } = null!;
        public string? ManagerName { get; set; }
        public string? Phone { get; set; }

        public ICollection<WarehouseInventory> WarehouseInventories { get; set; } = new List<WarehouseInventory>();

        private Warehouse() { }

        public Warehouse(
            Guid id,
            string name,
            string province,
            GeoLocation? location,
            string managerName,
            string? phone,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            Name = name;
            Province = province;
            Location = location;
            ManagerName = managerName;
            Phone = phone;
        }

    }
}