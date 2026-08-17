using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class InventoryTransactionConfiguration
    : IEntityTypeConfiguration<InventoryTransactionDataModel>
    {
        public void Configure(EntityTypeBuilder<InventoryTransactionDataModel> builder)
        {
            builder.ToTable("InventoryTransactions");

            builder.HasKey(x => x.TransactionId);

            builder.Property(x => x.TransactionId)
                .ValueGeneratedNever();

            builder.HasOne(x => x.Transaction)
                .WithOne(x => x.InventoryTransaction)
                .HasForeignKey<InventoryTransactionDataModel>(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Task)
                .WithMany(x => x.InventoryTransactions)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
