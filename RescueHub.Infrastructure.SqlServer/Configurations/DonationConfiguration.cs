using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class DonationConfiguration : IEntityTypeConfiguration<DonationDataModel>
    {
        public void Configure(EntityTypeBuilder<DonationDataModel> builder)
        {
            builder.ToTable("Donations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.DonationDate)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.ApprovedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.Remark)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasOne(x => x.Donator)
                .WithMany(x => x.Donations)
                .HasForeignKey(x => x.DonatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Approver)
                .WithMany()
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
