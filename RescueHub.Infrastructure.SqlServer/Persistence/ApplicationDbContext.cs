using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;

namespace RescueHub.Infrastructure.SqlServer.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đảm bảo số điện thoại là duy nhất (Unique)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // Cấu hình thuộc tính Point để ánh xạ chuẩn sang kiểu geography của SQL Server
            modelBuilder.Entity<User>()
                .Property(u => u.Location)
                .HasColumnType("geography");
        }
    }
}