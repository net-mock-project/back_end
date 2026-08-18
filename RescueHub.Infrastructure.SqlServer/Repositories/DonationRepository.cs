using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class DonationRepository : IDonationRepository
    {
        private readonly ApplicationDbContext _context;

        public DonationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<Warehouse>().Where(w => w.DeletedAt == null).ToListAsync(cancellationToken);
        }

        public async Task<Supply?> GetSupplyByNameAsync(string supplyName, CancellationToken cancellationToken)
        {
            return await _context.Set<Supply>()
                .FirstOrDefaultAsync(s => s.Name.ToLower() == supplyName.Trim().ToLower() && s.DeletedAt == null, cancellationToken);
        }

        public async Task AddSupplyAsync(Supply supply, CancellationToken cancellationToken)
        {
            await _context.Set<Supply>().AddAsync(supply, cancellationToken);
        }

        public async Task<WarehouseInventory?> GetWarehouseInventoryAsync(Guid warehouseId, Guid supplyId, CancellationToken cancellationToken)
        {
            return await _context.Set<WarehouseInventory>()
                .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && wi.SupplyId == supplyId, cancellationToken);
        }

        public async Task AddWarehouseInventoryAsync(WarehouseInventory warehouseInventory, CancellationToken cancellationToken)
        {
            await _context.Set<WarehouseInventory>().AddAsync(warehouseInventory, cancellationToken);
        }

        public async Task AddDonationAsync(Donation donation, CancellationToken cancellationToken)
        {
            await _context.Set<Donation>().AddAsync(donation, cancellationToken);
        }

        public async Task AddTransactionAsync(WarehouseTransaction transaction, CancellationToken cancellationToken)
        {
            await _context.Set<WarehouseTransaction>().AddAsync(transaction, cancellationToken);
        }

        public async Task AddDonationTransactionAsync(DonationTransaction donationTransaction, CancellationToken cancellationToken)
        {
            await _context.Set<DonationTransaction>().AddAsync(donationTransaction, cancellationToken);
        }

        public async Task<Donation?> GetDonationByIdAndUserIdAsync(Guid donationId, Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<Donation>()
                .Include(d => d.Donator)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Warehouses)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Supplys)
                .FirstOrDefaultAsync(d => d.Id == donationId && d.DonatorId == userId, cancellationToken);
        }

        public async Task<List<Donation>> GetDonationsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<Donation>()
                .Where(d => d.DonatorId == userId)
                .Include(d => d.Donator)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Warehouses)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Supplys)
                .ToListAsync(cancellationToken);
        }

        public async Task<WarehouseInventory?> GetWarehouseInventoryByIdAsync(Guid warehouseInventoryId, CancellationToken cancellationToken)
        {
            return await _context.Set<WarehouseInventory>()
                .FirstOrDefaultAsync(wi => wi.Id == warehouseInventoryId, cancellationToken);
        }

        public async Task<WarehouseTransaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            return await _context.Set<WarehouseTransaction>()
                .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
        }

        public async Task<List<WarehouseTransaction>> GetTransactionsByDonationIdAsync(Guid donationId, CancellationToken cancellationToken)
        {
            var transactionIds = await _context.Set<DonationTransaction>()
                .Where(dt => dt.DonationId == donationId)
                .Select(dt => dt.TransactionId)
                .ToListAsync(cancellationToken);

            return await _context.Set<WarehouseTransaction>()
                .Where(t => transactionIds.Contains(t.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Donation?> GetDonationByIdAsync(Guid donationId, CancellationToken cancellationToken)
        {
            return await _context.Set<Donation>()
                .Include(d => d.Donator)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Warehouses)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Supplys)
                .FirstOrDefaultAsync(d => d.Id == donationId, cancellationToken);
        }

        public async Task<Warehouse?> GetWarehouseByCoordinatorIdAsync(Guid coordinatorId, CancellationToken cancellationToken)
        {
            var coordinator = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Id == coordinatorId && u.DeletedAt == null, cancellationToken);

            if (coordinator == null || coordinator.Province == null)
            {
                return null;
            }

            string coordinatorProvince = coordinator.Province;

            return await _context.Set<Warehouse>()
                .FirstOrDefaultAsync(w => w.DeletedAt == null && w.Province != null && w.Province == coordinatorProvince, cancellationToken);
        }

        public async Task<List<Donation>> GetAllDonationsByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            return await _context.Set<Donation>()
                .Include(d => d.Donator)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Warehouses)
                .Include(d => d.DonationTransactions)
                    .ThenInclude(dt => dt.WarehouseTransactions)
                        .ThenInclude(t => t.WarehouseInventories)
                            .ThenInclude(wi => wi.Supplys)
                .Where(d => d.DonationTransactions.Any(dt => dt.WarehouseTransactions.WarehouseInventories.WarehouseId == warehouseId))
                .ToListAsync(cancellationToken);
        }
    }
}