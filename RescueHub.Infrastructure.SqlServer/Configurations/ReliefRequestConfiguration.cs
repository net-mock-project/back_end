using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class ReliefRequestConfiguration : IEntityTypeConfiguration<ReliefRequestDataModel>
    {
        public void Configure(EntityTypeBuilder<ReliefRequestDataModel> builder)
        {
            builder.ToTable("ReliefRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Location)
                .HasColumnType("geography")
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.ReliefImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.RequestedResource)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.StartTime)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.EndTime)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.UrgencyLevel)
                .IsRequired();

            builder.Property(x => x.EstimatedAffectedPeople)
                .IsRequired();

            builder.Property(x => x.EstimatedAffectedRadiusKm)
                .HasPrecision(10, 2);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CompletedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.RequesterId);
            builder.HasIndex(x => x.CoordinatorId);
            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Requester)
                .WithMany(x => x.ReliefRequests)
                .HasForeignKey(x => x.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Coordinator)
                .WithMany(x => x.CoordinatedReliefRequests)
                .HasForeignKey(x => x.CoordinatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
