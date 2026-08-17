using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogDataModel>
    {
        public void Configure(EntityTypeBuilder<AuditLogDataModel> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Action)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.EntityName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.OldValue)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValue)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => new
            {
                x.EntityName,
                x.EntityId
            });

            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne(x => x.User)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
