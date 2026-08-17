using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class ReliefTaskConfiguration : IEntityTypeConfiguration<ReliefTaskDataModel>
    {
        public void Configure(EntityTypeBuilder<ReliefTaskDataModel> builder)
        {
            builder.ToTable("ReliefTasks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Location)
                .HasColumnType("geography");

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.RequestId);
            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Request)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
