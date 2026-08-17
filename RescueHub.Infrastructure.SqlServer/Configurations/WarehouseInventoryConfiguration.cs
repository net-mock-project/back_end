using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class WarehouseInventoryConfiguration
    : IEntityTypeConfiguration<WarehouseInventoryDataModel>
    {
        public void Configure(EntityTypeBuilder<WarehouseInventoryDataModel> builder)
        {
            builder.ToTable("WarehouseInventories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => new
            {
                x.WarehouseId,
                x.SupplyId
            }).IsUnique();

            builder.HasOne(x => x.Warehouse)
                .WithMany(x => x.Inventories)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Supply)
                .WithMany(x => x.WarehouseInventories)
                .HasForeignKey(x => x.SupplyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
