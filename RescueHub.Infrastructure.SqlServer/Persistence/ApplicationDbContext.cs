using Microsoft.EntityFrameworkCore;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RoleDataModel> Roles => Set<RoleDataModel>();
        public DbSet<UserDataModel> Users => Set<UserDataModel>();
        public DbSet<VolunteerDataModel> Volunteers => Set<VolunteerDataModel>();
        public DbSet<SkillDataModel> Skills => Set<SkillDataModel>();
        public DbSet<VolunteerSkillDataModel> VolunteerSkills => Set<VolunteerSkillDataModel>();

        public DbSet<ReliefRequestDataModel> ReliefRequests => Set<ReliefRequestDataModel>();
        public DbSet<ReliefTaskDataModel> ReliefTasks => Set<ReliefTaskDataModel>();
        public DbSet<TaskSkillDataModel> TaskSkills => Set<TaskSkillDataModel>();

        public DbSet<VolunteerEngagementDataModel> VolunteerEngagements
            => Set<VolunteerEngagementDataModel>();

        public DbSet<TaskAssignmentDataModel> TaskAssignments
            => Set<TaskAssignmentDataModel>();

        public DbSet<WarehouseDataModel> Warehouses => Set<WarehouseDataModel>();
        public DbSet<SupplyDataModel> Supplies => Set<SupplyDataModel>();

        public DbSet<WarehouseInventoryDataModel> WarehouseInventories
            => Set<WarehouseInventoryDataModel>();

        public DbSet<WarehouseTransactionDataModel> WarehouseTransactions
            => Set<WarehouseTransactionDataModel>();

        public DbSet<InventoryTransactionDataModel> InventoryTransactions
            => Set<InventoryTransactionDataModel>();

        public DbSet<DonationDataModel> Donations => Set<DonationDataModel>();

        public DbSet<DonationTransactionDataModel> DonationTransactions
            => Set<DonationTransactionDataModel>();

        public DbSet<NotificationDataModel> Notifications
            => Set<NotificationDataModel>();

        public DbSet<AuditLogDataModel> AuditLogs
            => Set<AuditLogDataModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tự động áp dụng các Configuration trong project
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}