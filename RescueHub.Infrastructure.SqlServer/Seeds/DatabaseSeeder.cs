using RescueHub.Infrastructure.SqlServer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RescueHub.Infrastructure.SqlServer.Seeds
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.Database.MigrateAsync(cancellationToken);

            await SeedRolesAsync(cancellationToken);
            await SeedSkillsAsync(cancellationToken);
            await SeedSuppliesAsync(cancellationToken);


            await SeedUsersAsync(cancellationToken);
            await SeedVolunteersAsync(cancellationToken);
            await SeedVolunteerSkillsAsync(cancellationToken);

            await SeedWarehousesAsync(cancellationToken);
            await SeedWarehouseInventoriesAsync(cancellationToken);
            await SeedWarehouseTransactionsAsync(cancellationToken);

            await SeedReliefRequestsAsync(cancellationToken);
            await SeedReliefTasksAsync(cancellationToken);
            await SeedTaskSkillsAsync(cancellationToken);
            await SeedVolunteerEngagementsAsync(cancellationToken);
            await SeedTaskAssignmentsAsync(cancellationToken);

            await SeedDonationsAsync(cancellationToken);
            await SeedDonationTransactionsAsync(cancellationToken);
            await SeedInventoryTransactionsAsync(cancellationToken);

            await SeedNotificationsAsync(cancellationToken);
            await SeedAuditLogsAsync(cancellationToken);
        }

        private async Task SeedRolesAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Roles.AnyAsync(cancellationToken))
                return;

            await _context.Roles.AddRangeAsync(
                SeedData.Roles(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedSkillsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Skills.AnyAsync(cancellationToken))
                return;

            await _context.Skills.AddRangeAsync(
                SeedData.Skills(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedSuppliesAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Supplies.AnyAsync(cancellationToken))
                return;

            await _context.Supplies.AddRangeAsync(
                SeedData.Supplies(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedUsersAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(cancellationToken))
                return;

            await _context.Users.AddRangeAsync(
                SeedData.Users(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedVolunteersAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Volunteers.AnyAsync(cancellationToken))
                return;

            await _context.Volunteers.AddRangeAsync(
                SeedData.Volunteers(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedVolunteerSkillsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.VolunteerSkills.AnyAsync(cancellationToken))
                return;

            await _context.VolunteerSkills.AddRangeAsync(
                SeedData.VolunteerSkills(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedWarehousesAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Warehouses.AnyAsync(cancellationToken))
                return;

            await _context.Warehouses.AddRangeAsync(
                SeedData.Warehouses(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedWarehouseInventoriesAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.WarehouseInventories.AnyAsync(cancellationToken))
                return;

            await _context.WarehouseInventories.AddRangeAsync(
                SeedData.WarehouseInventories(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedWarehouseTransactionsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.WarehouseTransactions.AnyAsync(cancellationToken))
                return;

            await _context.WarehouseTransactions.AddRangeAsync(
                SeedData.WarehouseTransactions(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedReliefRequestsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.ReliefRequests.AnyAsync(cancellationToken))
                return;

            await _context.ReliefRequests.AddRangeAsync(
                SeedData.ReliefRequests(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedReliefTasksAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.ReliefTasks.AnyAsync(cancellationToken))
                return;

            await _context.ReliefTasks.AddRangeAsync(
                SeedData.ReliefTasks(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedTaskSkillsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.TaskSkills.AnyAsync(cancellationToken))
                return;

            await _context.TaskSkills.AddRangeAsync(
                SeedData.TaskSkills(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedVolunteerEngagementsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.VolunteerEngagements.AnyAsync(cancellationToken))
                return;

            await _context.VolunteerEngagements.AddRangeAsync(
                SeedData.VolunteerEngagements(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedTaskAssignmentsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.TaskAssignments.AnyAsync(cancellationToken))
                return;

            await _context.TaskAssignments.AddRangeAsync(
                SeedData.TaskAssignments(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedDonationsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Donations.AnyAsync(cancellationToken))
                return;

            await _context.Donations.AddRangeAsync(
                SeedData.Donations(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedDonationTransactionsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.DonationTransactions.AnyAsync(cancellationToken))
                return;

            await _context.DonationTransactions.AddRangeAsync(
                SeedData.DonationTransactions(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedInventoryTransactionsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.InventoryTransactions.AnyAsync(cancellationToken))
                return;

            await _context.InventoryTransactions.AddRangeAsync(
                SeedData.InventoryTransactions(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedNotificationsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.Notifications.AnyAsync(cancellationToken))
                return;

            await _context.Notifications.AddRangeAsync(
                SeedData.Notifications(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedAuditLogsAsync(
            CancellationToken cancellationToken)
        {
            if (await _context.AuditLogs.AnyAsync(cancellationToken))
                return;

            await _context.AuditLogs.AddRangeAsync(
                SeedData.AuditLogs(),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
