using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class DonationTransactionConfiguration
    : IEntityTypeConfiguration<DonationTransactionDataModel>
    {
        public void Configure(EntityTypeBuilder<DonationTransactionDataModel> builder)
        {
            builder.ToTable("DonationTransactions");

            builder.HasKey(x => x.TransactionId);

            builder.Property(x => x.TransactionId)
                .ValueGeneratedNever();

            builder.HasOne(x => x.Transaction)
                .WithOne(x => x.DonationTransaction)
                .HasForeignKey<DonationTransactionDataModel>(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Donation)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.DonationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
