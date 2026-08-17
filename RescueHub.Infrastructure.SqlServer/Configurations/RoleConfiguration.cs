using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<RoleDataModel>
    {
        public void Configure(EntityTypeBuilder<RoleDataModel> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(250);

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