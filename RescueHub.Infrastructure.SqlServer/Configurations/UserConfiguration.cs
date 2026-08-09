using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Domain.Entities;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Location lưu bằng kiểu geography trong SQL Server
            builder.Property(x => x.Location)
                .HasColumnType("geography");
        }
    }
}