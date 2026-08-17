using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class WarehouseTransactionConfiguration
    : IEntityTypeConfiguration<WarehouseTransactionDataModel>
    {
        public void Configure(EntityTypeBuilder<WarehouseTransactionDataModel> builder)
        {
            builder.ToTable("WarehouseTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.TransactionType)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.WarehouseInventoryId);
            builder.HasIndex(x => x.CreatedBy);
            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne(x => x.WarehouseInventory)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.WarehouseInventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
