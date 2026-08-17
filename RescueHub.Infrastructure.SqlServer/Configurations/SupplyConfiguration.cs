using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class SupplyConfiguration : IEntityTypeConfiguration<SupplyDataModel>
    {
        public void Configure(EntityTypeBuilder<SupplyDataModel> builder)
        {
            builder.ToTable("Supplies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Category)
                .HasMaxLength(100);

            builder.Property(x => x.Unit)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.MinimumStock)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
