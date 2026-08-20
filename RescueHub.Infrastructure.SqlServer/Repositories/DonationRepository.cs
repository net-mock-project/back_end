using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Infrastructure.SqlServer.Models;
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
            var model = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null, cancellationToken);

            if (model == null) return null;

            return new User(
                model.Id,
                model.RoleId,
                model.Location != null ? new GeoLocation(model.Location.Y, model.Location.X) : null,
                model.Province,
                model.ProfileUrl,
                model.FullName,
                model.Email,
                model.Phone,
                model.DateOfBirth,
                model.Gender,
                model.PasswordHash,
                model.Status,
                model.IsVerified,
                model.CreatedAt,
                model.UpdatedAt,
                model.DeletedAt
            );
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken cancellationToken)
        {
            var models = await _context.Warehouses
                .Where(w => w.DeletedAt == null)
                .ToListAsync(cancellationToken);

            return models.Select(w => new Warehouse(
                w.Id,
                w.Name,
                w.Province,
                w.Location != null ? new GeoLocation(w.Location.Y, w.Location.X) : null,
                w.ManagerName ?? string.Empty,
                w.Phone,
                w.CreatedAt,
                w.UpdatedAt,
                w.DeletedAt
            )).ToList();
        }

        public async Task<Supply?> GetSupplyByNameAsync(string supplyName, CancellationToken cancellationToken)
        {
            var model = await _context.Supplies
                .FirstOrDefaultAsync(s => s.Name.ToLower() == supplyName.Trim().ToLower(), cancellationToken);

            if (model == null) return null;

            return new Supply(
                model.Id,
                model.Name,
                model.Category ?? string.Empty,
                model.Unit,
                model.MinimumStock,
                model.CreatedAt,
                model.UpdatedAt,
                null
            );
        }

        public async Task AddSupplyAsync(Supply supply, CancellationToken cancellationToken)
        {
            var model = new SupplyDataModel
            {
                Id = supply.Id,
                Name = supply.Name,
                Category = supply.Category,
                Unit = supply.Unit,
                MinimumStock = supply.MinimumStock,
                CreatedAt = supply.CreatedAt,
                UpdatedAt = supply.UpdatedAt
            };
            await _context.Supplies.AddAsync(model, cancellationToken);
        }

        public async Task<WarehouseInventory?> GetWarehouseInventoryAsync(Guid warehouseId, Guid supplyId, CancellationToken cancellationToken)
        {
            var model = await _context.WarehouseInventories
                .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && wi.SupplyId == supplyId, cancellationToken);

            if (model == null) return null;

            return new WarehouseInventory(
                model.Id,
                model.WarehouseId,
                model.SupplyId,
                model.Quantity,
                model.CreatedAt,
                model.UpdatedAt,
                null
            );
        }

        public async Task AddWarehouseInventoryAsync(WarehouseInventory warehouseInventory, CancellationToken cancellationToken)
        {
            var model = new WarehouseInventoryDataModel
            {
                Id = warehouseInventory.Id,
                WarehouseId = warehouseInventory.WarehouseId,
                SupplyId = warehouseInventory.SupplyId,
                Quantity = warehouseInventory.Quantity,
                CreatedAt = warehouseInventory.CreatedAt,
                UpdatedAt = warehouseInventory.UpdatedAt
            };
            await _context.WarehouseInventories.AddAsync(model, cancellationToken);
        }

        public async Task AddDonationAsync(Donation donation, CancellationToken cancellationToken)
        {
            var model = new DonationDataModel
            {
                Id = donation.Id,
                DonatorId = donation.DonatorId,
                Status = donation.Status,
                DonationDate = donation.DonationDate,
                ApprovedBy = donation.ApprovedBy,
                ApprovedAt = donation.ApprovedAt,
                Remark = donation.Remark,
                CreatedAt = donation.CreatedAt,
                UpdatedAt = donation.UpdatedAt,
                DeletedAt = donation.DeletedAt
            };
            await _context.Donations.AddAsync(model, cancellationToken);
        }

        public async Task AddTransactionAsync(WarehouseTransaction transaction, CancellationToken cancellationToken)
        {
            var model = new WarehouseTransactionDataModel
            {
                Id = transaction.Id,
                WarehouseInventoryId = transaction.WarehouseInventoryId,
                Quantity = transaction.Quantity,
                TransactionType = transaction.TransactionType,
                Status = transaction.Status,
                CreatedBy = transaction.CreatedBy,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt,
                DeletedAt = transaction.DeletedAt
            };
            await _context.WarehouseTransactions.AddAsync(model, cancellationToken);
        }

        public async Task AddDonationTransactionAsync(DonationTransaction donationTransaction, CancellationToken cancellationToken)
        {
            var model = new DonationTransactionDataModel
            {
                DonationId = donationTransaction.DonationId,
                TransactionId = donationTransaction.TransactionId
            };
            await _context.DonationTransactions.AddAsync(model, cancellationToken);
        }

        public async Task<Donation?> GetDonationByIdAndUserIdAsync(Guid donationId, Guid userId, CancellationToken cancellationToken)
        {
            var model = await _context.Donations
                .Include(d => d.Donator)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Warehouse)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Supply)
                .FirstOrDefaultAsync(d => d.Id == donationId && d.DonatorId == userId, cancellationToken);

            if (model == null) return null;

            return MapToDonationDomain(model);
        }

        public async Task<List<Donation>> GetDonationsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var models = await _context.Donations
                .Where(d => d.DonatorId == userId)
                .Include(d => d.Donator)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Warehouse)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Supply)
                .ToListAsync(cancellationToken);

            return models.Select(MapToDonationDomain).ToList();
        }

        public async Task<WarehouseInventory?> GetWarehouseInventoryByIdAsync(Guid warehouseInventoryId, CancellationToken cancellationToken)
        {
            var model = await _context.WarehouseInventories
                .FirstOrDefaultAsync(wi => wi.Id == warehouseInventoryId, cancellationToken);

            if (model == null) return null;

            return new WarehouseInventory(
                model.Id,
                model.WarehouseId,
                model.SupplyId,
                model.Quantity,
                model.CreatedAt,
                model.UpdatedAt,
                null
            );
        }

        public async Task<WarehouseTransaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            var model = await _context.WarehouseTransactions
                .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

            if (model == null) return null;

            return new WarehouseTransaction
            {
                Id = model.Id,
                WarehouseInventoryId = model.WarehouseInventoryId,
                Quantity = model.Quantity,
                TransactionType = model.TransactionType,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                DeletedAt = model.DeletedAt
            };
        }

        public async Task<List<WarehouseTransaction>> GetTransactionsByDonationIdAsync(Guid donationId, CancellationToken cancellationToken)
        {
            var transactionIds = await _context.DonationTransactions
                .Where(dt => dt.DonationId == donationId)
                .Select(dt => dt.TransactionId)
                .ToListAsync(cancellationToken);

            var models = await _context.WarehouseTransactions
                .Where(t => transactionIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            return models.Select(t => new WarehouseTransaction
            {
                Id = t.Id,
                WarehouseInventoryId = t.WarehouseInventoryId,
                Quantity = t.Quantity,
                TransactionType = t.TransactionType,
                Status = t.Status,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt
            }).ToList();
        }

        public async Task<Donation?> GetDonationByIdAsync(Guid donationId, CancellationToken cancellationToken)
        {
            var model = await _context.Donations
                .Include(d => d.Donator)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Warehouse)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Supply)
                .FirstOrDefaultAsync(d => d.Id == donationId, cancellationToken);

            if (model == null) return null;

            return MapToDonationDomain(model);
        }

        public async Task<Warehouse?> GetWarehouseByCoordinatorIdAsync(Guid coordinatorId, CancellationToken cancellationToken)
        {
            var coordinator = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == coordinatorId && u.DeletedAt == null, cancellationToken);

            if (coordinator == null || coordinator.Province == null)
            {
                return null;
            }

            string coordinatorProvince = coordinator.Province;

            var warehouseModel = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.DeletedAt == null && w.Province != null && w.Province == coordinatorProvince, cancellationToken);

            if (warehouseModel == null) return null;

            return new Warehouse(
                warehouseModel.Id,
                warehouseModel.Name,
                warehouseModel.Province,
                warehouseModel.Location != null ? new GeoLocation(warehouseModel.Location.Y, warehouseModel.Location.X) : null,
                warehouseModel.ManagerName ?? string.Empty,
                warehouseModel.Phone,
                warehouseModel.CreatedAt,
                warehouseModel.UpdatedAt,
                warehouseModel.DeletedAt
            );
        }

        public async Task<List<Donation>> GetAllDonationsByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            var models = await _context.Donations
                .Include(d => d.Donator)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Warehouse)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Supply)
                .Where(d => d.Transactions.Any(dt => dt.Transaction.WarehouseInventory.WarehouseId == warehouseId))
                .ToListAsync(cancellationToken);

            return models.Select(MapToDonationDomain).ToList();
        }


        // CÁC HÀM TRACKING & CẬP NHẬT


        public async Task<Donation?> GetDonationWithTrackingAsync(Guid donationId, CancellationToken cancellationToken)
        {
            var model = await _context.Donations
                .Include(d => d.Donator)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Warehouse)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Supply)
                .FirstOrDefaultAsync(d => d.Id == donationId, cancellationToken);

            return model != null ? MapToDonationDomain(model) : null;
        }

        public async Task<Donation?> GetDonationWithTrackingByIdAndUserIdAsync(Guid donationId, Guid userId, CancellationToken cancellationToken)
        {
            var model = await _context.Donations
                .Include(d => d.Donator)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Warehouse)
                .Include(d => d.Transactions)
                    .ThenInclude(dt => dt.Transaction)
                        .ThenInclude(t => t.WarehouseInventory)
                            .ThenInclude(wi => wi.Supply)
                .FirstOrDefaultAsync(d => d.Id == donationId && d.DonatorId == userId, cancellationToken);

            return model != null ? MapToDonationDomain(model) : null;
        }

        public async Task<WarehouseInventory?> GetWarehouseInventoryWithTrackingAsync(Guid warehouseInventoryId, CancellationToken cancellationToken)
        {
            var model = await _context.WarehouseInventories
                .FirstOrDefaultAsync(wi => wi.Id == warehouseInventoryId, cancellationToken);

            if (model == null) return null;

            return new WarehouseInventory(
                model.Id,
                model.WarehouseId,
                model.SupplyId,
                model.Quantity,
                model.CreatedAt,
                model.UpdatedAt,
                null
            );
        }

        public async Task<WarehouseTransaction?> GetTransactionWithTrackingAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            var model = await _context.WarehouseTransactions
                .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

            if (model == null) return null;

            return new WarehouseTransaction
            {
                Id = model.Id,
                WarehouseInventoryId = model.WarehouseInventoryId,
                Quantity = model.Quantity,
                TransactionType = model.TransactionType,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                DeletedAt = model.DeletedAt
            };
        }

        public void UpdateDonation(Donation donation)
        {
            // Map từ Domain Entity ngược lại thành DataModel
            var model = _context.Donations.Find(donation.Id);
            if (model != null)
            {
                model.Status = donation.Status;
                model.ApprovedBy = donation.ApprovedBy;
                model.ApprovedAt = donation.ApprovedAt;
                model.Remark = donation.Remark;
                model.UpdatedAt = donation.UpdatedAt;
                model.DeletedAt = donation.DeletedAt;
                _context.Donations.Update(model);
            }
        }

        public void RemoveDonationTransaction(DonationTransaction donationTransaction)
        {
            var model = _context.DonationTransactions
                .FirstOrDefault(dt => dt.DonationId == donationTransaction.DonationId && dt.TransactionId == donationTransaction.TransactionId);

            if (model != null)
            {
                _context.DonationTransactions.Remove(model);
            }
        }

        public void UpdateWarehouseInventory(WarehouseInventory inventory)
        {
            var model = _context.WarehouseInventories.Find(inventory.Id);
            if (model != null)
            {
                model.Quantity = inventory.Quantity;
                model.UpdatedAt = inventory.UpdatedAt;
                _context.WarehouseInventories.Update(model);
            }
        }

        
        // Helper method để map DonationDataModel sang Donation Domain Entity
        private static Donation MapToDonationDomain(DonationDataModel model)
        {
            var donation = new Donation(
                model.Id,
                model.DonatorId,
                model.Status,
                model.DonationDate,
                model.ApprovedBy,
                model.ApprovedAt,
                model.Remark,
                model.CreatedAt,
                model.UpdatedAt,
                model.DeletedAt
            );

            // 1. Map thông tin Donator (User) nếu có
            if (model.Donator != null)
            {
                donation.Donator = new User(
                    model.Donator.Id,
                    model.Donator.RoleId,
                    model.Donator.Location != null ? new GeoLocation(model.Donator.Location.Y, model.Donator.Location.X) : null,
                    model.Donator.Province,
                    model.Donator.ProfileUrl,
                    model.Donator.FullName,
                    model.Donator.Email,
                    model.Donator.Phone,
                    model.Donator.DateOfBirth,
                    model.Donator.Gender,
                    model.Donator.PasswordHash,
                    model.Donator.Status,
                    model.Donator.IsVerified,
                    model.Donator.CreatedAt,
                    model.Donator.UpdatedAt,
                    model.Donator.DeletedAt
                );
            }

            // 2. Map danh sách DonationTransactions và các tầng bên trong nếu có
            if (model.Transactions != null && model.Transactions.Any())
            {
                foreach (var dtModel in model.Transactions)
                {
                    var donationTransaction = new DonationTransaction(dtModel.DonationId, dtModel.TransactionId);

                    if (dtModel.Transaction != null)
                    {
                        donationTransaction.WarehouseTransactions = new WarehouseTransaction
                        {
                            Id = dtModel.Transaction.Id,
                            WarehouseInventoryId = dtModel.Transaction.WarehouseInventoryId,
                            Quantity = dtModel.Transaction.Quantity,
                            TransactionType = dtModel.Transaction.TransactionType,
                            Status = dtModel.Transaction.Status,
                            CreatedBy = dtModel.Transaction.CreatedBy,
                            CreatedAt = dtModel.Transaction.CreatedAt,
                            UpdatedAt = dtModel.Transaction.UpdatedAt,
                            DeletedAt = dtModel.Transaction.DeletedAt,
                        };

                        if (dtModel.Transaction.WarehouseInventory != null)
                        {
                            var wiModel = dtModel.Transaction.WarehouseInventory;

                            donationTransaction.WarehouseTransactions.WarehouseInventories = new WarehouseInventory(
                                wiModel.Id,
                                wiModel.WarehouseId,
                                wiModel.SupplyId,
                                wiModel.Quantity,
                                wiModel.CreatedAt,
                                wiModel.UpdatedAt,
                                null
                            );

                            // Map Warehouse
                            if (wiModel.Warehouse != null)
                            {
                                donationTransaction.WarehouseTransactions.WarehouseInventories.Warehouses = new Warehouse(
                                    wiModel.Warehouse.Id,
                                    wiModel.Warehouse.Name,
                                    wiModel.Warehouse.Province,
                                    wiModel.Warehouse.Location != null ? new GeoLocation(wiModel.Warehouse.Location.Y, wiModel.Warehouse.Location.X) : null,
                                    wiModel.Warehouse.ManagerName ?? string.Empty,
                                    wiModel.Warehouse.Phone,
                                    wiModel.Warehouse.CreatedAt,
                                    wiModel.Warehouse.UpdatedAt,
                                    wiModel.Warehouse.DeletedAt
                                );
                            }

                            // Map Supply
                            if (wiModel.Supply != null)
                            {
                                donationTransaction.WarehouseTransactions.WarehouseInventories.Supplys = new Supply(
                                    wiModel.Supply.Id,
                                    wiModel.Supply.Name,
                                    wiModel.Supply.Category ?? string.Empty,
                                    wiModel.Supply.Unit,
                                    wiModel.Supply.MinimumStock,
                                    wiModel.Supply.CreatedAt,
                                    wiModel.Supply.UpdatedAt,
                                    null
                                );
                            }
                        }
                    }

                    donation.DonationTransactions.Add(donationTransaction);
                }
            }

            return donation;
        }
    }
}