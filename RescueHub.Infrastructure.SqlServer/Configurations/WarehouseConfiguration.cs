using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<WarehouseDataModel>
    {
        public void Configure(EntityTypeBuilder<WarehouseDataModel> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Location)
                .HasColumnType("geography")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ManagerName)
                .HasMaxLength(150);

            builder.Property(x => x.Province)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Phone)
                .HasMaxLength(20);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.Province);
        }
    }
}
