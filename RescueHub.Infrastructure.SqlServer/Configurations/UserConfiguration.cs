using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserDataModel>
    {
        public void Configure(EntityTypeBuilder<UserDataModel> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Gender)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.DateOfBirth)
                .HasColumnType("date");

            builder.Property(x => x.Location)
                .HasColumnType("geography");

            builder.Property(x => x.Province)
                .HasMaxLength(100);

            builder.Property(x => x.ProfileUrl)
                .HasMaxLength(500);

            builder.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.IsVerified)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.HasIndex(x => x.Phone)
                .IsUnique();

            builder.HasIndex(x => x.RoleId);

            builder.HasIndex(x => x.Province);

            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Volunteer)
                .WithOne(x => x.User)
                .HasForeignKey<VolunteerDataModel>(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}