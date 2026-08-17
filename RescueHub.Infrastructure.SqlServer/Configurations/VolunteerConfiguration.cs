using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<VolunteerDataModel>
    {
        public void Configure(EntityTypeBuilder<VolunteerDataModel> builder)
        {
            builder.ToTable("Volunteers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.ExperienceYears)
                .IsRequired();

            builder.Property(x => x.ApprovalStatus)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CVUrl)
                .HasMaxLength(500);

            builder.Property(x => x.ApprovedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.ApprovalStatus);

            builder.HasOne(x => x.User)
                .WithOne(x => x.Volunteer)
                .HasForeignKey<VolunteerDataModel>(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Approver)
                .WithMany()
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
