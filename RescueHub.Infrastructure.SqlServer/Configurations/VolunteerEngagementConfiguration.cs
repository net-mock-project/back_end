using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class VolunteerEngagementConfiguration
    : IEntityTypeConfiguration<VolunteerEngagementDataModel>
    {
        public void Configure(EntityTypeBuilder<VolunteerEngagementDataModel> builder)
        {
            builder.ToTable("VolunteerEngagements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.PerformanceAssessment)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => new
            {
                x.VolunteerId,
                x.RequestId
            }).IsUnique();

            builder.HasOne(x => x.Volunteer)
                .WithMany(x => x.Engagements)
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Request)
                .WithMany(x => x.VolunteerEngagements)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
