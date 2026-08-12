using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    // Cấu hình bảng User trong SQL Server
    public class UserConfiguration
        : IEntityTypeConfiguration<UserDataModel>
    {
        public void Configure(EntityTypeBuilder<UserDataModel> builder)
        {
            builder.ToTable("User");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.RoleId)
                .IsRequired();

            // Location lưu dưới dạng geography
            builder.Property(u => u.Location)
                .HasColumnType("geography");

            builder.Property(u => u.FullName)
                .IsRequired();

            builder.Property(u => u.Email)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Status)
                .IsRequired();

            builder.Property(u => u.IsVerified)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .IsRequired();
        }
    }
}