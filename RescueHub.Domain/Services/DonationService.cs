using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Donations;

namespace RescueHub.Domain.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepository;

        public DonationService(IDonationRepository donationRepository)
        {
            _donationRepository = donationRepository;
        }

        public async Task<Donation> CreateDonationAsync(
            Guid donatorId,
            List<(string SupplyName, int Quantity, string Unit)> items,
            DateTime donationDate,
            CancellationToken cancellationToken)
        {
            if (items == null || !items.Any())
            {
                throw new Exception("At least one donation item is required.");
            }

            var user = await _donationRepository.GetUserByIdAsync(donatorId, cancellationToken);
            if (user == null || user.Location == null)
            {
                throw new Exception($"User '{donatorId}' not found or location is not set.");
            }

            var warehouses = await _donationRepository.GetAllWarehousesAsync(cancellationToken);
            if (warehouses == null || !warehouses.Any())
            {
                throw new Exception("No available warehouses found.");
            }

            Warehouse? nearestWarehouse = null;
            double minDistance = double.MaxValue;

            foreach (var warehouse in warehouses)
            {
                if (warehouse.Location == null) continue;

                double distance = CalculateDistance(
                    user.Location.Latitude, user.Location.Longitude,
                    warehouse.Location.Latitude, warehouse.Location.Longitude);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestWarehouse = warehouse;
                }
            }

            if (nearestWarehouse == null)
            {
                throw new Exception("No warehouse with valid location found nearby.");
            }

            var donationId = Guid.NewGuid();
            var donation = new Donation(
                id: donationId,
                donatorId: donatorId,
                status: DonationStatus.Pending,
                donationDate: donationDate,
                approvedBy: null,
                approvedAt: null,
                remark: null,
                createdAt: DateTime.UtcNow,
                updatedAt: null,
                deletedAt: null
            );
            await _donationRepository.AddDonationAsync(donation, cancellationToken);

            foreach (var item in items)
            {
                var supply = await _donationRepository.GetSupplyByNameAsync(item.SupplyName, cancellationToken);
                Guid supplyId;
                if (supply != null)
                {
                    supplyId = supply.Id;
                }
                else
                {
                    supplyId = Guid.NewGuid();
                    supply = new Supply(
                        id: supplyId,
                        name: item.SupplyName,
                        category: "General",
                        unit: item.Unit,
                        minimumStock: 10,
                        createdAt: DateTime.UtcNow,
                        updatedAt: null,
                        deletedAt: null
                    );
                    await _donationRepository.AddSupplyAsync(supply, cancellationToken);
                }

                var warehouseInventory = await _donationRepository.GetWarehouseInventoryAsync(nearestWarehouse.Id, supplyId, cancellationToken);
                Guid warehouseInventoryId;
                if (warehouseInventory == null)
                {
                    warehouseInventoryId = Guid.NewGuid();
                    warehouseInventory = new WarehouseInventory(
                        id: warehouseInventoryId,
                        warehouseId: nearestWarehouse.Id,
                        supplyId: supplyId,
                        quantity: 0,
                        createdAt: DateTime.UtcNow,
                        updatedAt: null,
                        deletedAt: null
                    );
                    await _donationRepository.AddWarehouseInventoryAsync(warehouseInventory, cancellationToken);
                }
                else
                {
                    warehouseInventoryId = warehouseInventory.Id;
                }

                var transactionId = Guid.NewGuid();
                var transaction = new WarehouseTransaction
                {
                    Id = transactionId,
                    WarehouseInventoryId = warehouseInventoryId,
                    Quantity = item.Quantity,
                    TransactionType = WarehouseTransactionType.Import,
                    Status = WarehouseTransactionStatus.Pending,
                    CreatedBy = donatorId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    DeletedAt = null
                };
                await _donationRepository.AddTransactionAsync(transaction, cancellationToken);

                var donationTransaction = new DonationTransaction(donationId, transactionId);
                await _donationRepository.AddDonationTransactionAsync(donationTransaction, cancellationToken);
            }

            return donation;
        }

        public async Task<Donation?> UpdateDonationAsync(
            Guid userId,
            Guid donationId,
            List<(string SupplyName, int Quantity, string Unit)>? items,
            DateTime? donationDate,
            CancellationToken cancellationToken)
        {
            var donation = await _donationRepository.GetDonationWithTrackingAsync(donationId, cancellationToken);
            if (donation == null || donation.DonatorId != userId) return null;

            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể chỉnh sửa đơn quyên góp khi đang ở trạng thái chờ xử lý (Pending).");
            }

            if (donationDate.HasValue)
            {
                donation.UpdateDate(donationDate.Value); 
            }

            if (items != null && items.Any())
            {
                var firstDt = donation.DonationTransactions.FirstOrDefault();
                if (firstDt?.WarehouseTransactions?.WarehouseInventories == null)
                {
                    throw new Exception("Không tìm thấy thông tin kho liên kết với đơn quyên góp này.");
                }

                Guid warehouseId = firstDt.WarehouseTransactions.WarehouseInventories.WarehouseId;

                foreach (var dt in donation.DonationTransactions.ToList())
                {
                    _donationRepository.RemoveDonationTransaction(dt);
                    donation.DonationTransactions.Remove(dt);
                }

                foreach (var item in items)
                {
                    var supply = await _donationRepository.GetSupplyByNameAsync(item.SupplyName, cancellationToken);
                    Guid supplyId;
                    if (supply != null)
                    {
                        supplyId = supply.Id;
                    }
                    else
                    {
                        supplyId = Guid.NewGuid();
                        supply = new Supply(
                            id: supplyId,
                            name: item.SupplyName,
                            category: "General",
                            unit: item.Unit,
                            minimumStock: 10,
                            createdAt: DateTime.UtcNow,
                            updatedAt: null,
                            deletedAt: null
                        );
                        await _donationRepository.AddSupplyAsync(supply, cancellationToken);
                    }

                    var warehouseInventory = await _donationRepository.GetWarehouseInventoryAsync(warehouseId, supplyId, cancellationToken);
                    Guid warehouseInventoryId;
                    if (warehouseInventory == null)
                    {
                        warehouseInventoryId = Guid.NewGuid();
                        var newInventoryDomain = new WarehouseInventory(
                            id: warehouseInventoryId,
                            warehouseId: warehouseId,
                            supplyId: supplyId,
                            quantity: 0,
                            createdAt: DateTime.UtcNow,
                            updatedAt: null,
                            deletedAt: null
                        );
                        await _donationRepository.AddWarehouseInventoryAsync(newInventoryDomain, cancellationToken);
                    }
                    else
                    {
                        warehouseInventoryId = warehouseInventory.Id;
                    }

                    var transactionId = Guid.NewGuid();
                    var newTransaction = new WarehouseTransaction
                    {
                        Id = transactionId,
                        WarehouseInventoryId = warehouseInventoryId,
                        Quantity = item.Quantity,
                        TransactionType = WarehouseTransactionType.Import,
                        Status = WarehouseTransactionStatus.Pending,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null,
                        DeletedAt = null
                    };
                    await _donationRepository.AddTransactionAsync(newTransaction, cancellationToken);

                    var donationTransaction = new DonationTransaction(donation.Id, transactionId);
                    await _donationRepository.AddDonationTransactionAsync(donationTransaction, cancellationToken);
                }
            }

            _donationRepository.UpdateDonation(donation);

            return await _donationRepository.GetDonationByIdAndUserIdAsync(donationId, userId, cancellationToken);
        }

        public async Task<List<string>> GetSuppliesNameAsync(CancellationToken cancellationToken) 
        {
            var supplies = await _donationRepository.GetAllSuppliesAsync(cancellationToken);
            return supplies.Select(s => s.Name).ToList();
        }

        public async Task<bool> CancelDonationAsync(Guid userId, Guid donationId, CancellationToken cancellationToken)
        {
            var donation = await _donationRepository.GetDonationWithTrackingAsync(donationId, cancellationToken);
            if (donation == null || donation.DonatorId != userId) return false;

            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Không thể hủy đơn quyên góp đã được duyệt hoặc đã hoàn thành.");
            }

            donation.Cancel(); 
            _donationRepository.UpdateDonation(donation);
            return true;
        }

        public async Task<List<Donation>> GetDonationsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _donationRepository.GetDonationsByUserIdAsync(userId, cancellationToken);
        }

        public async Task<bool> ConfirmDonationReceivedAsync(Guid donationId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var donation = await _donationRepository.GetDonationWithTrackingAsync(donationId, cancellationToken);
            if (donation == null) return false;

            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể xác nhận nhận đồ cho các đơn đang ở trạng thái Pending.");
            }

            donation.Complete(coordinatorId); 
            _donationRepository.UpdateDonation(donation);

            foreach (var dt in donation.DonationTransactions)
            {
                if (dt.WarehouseTransactions != null)
                {
                    var transaction = dt.WarehouseTransactions;
                    var inventory = await _donationRepository.GetWarehouseInventoryWithTrackingAsync(transaction.WarehouseInventoryId, cancellationToken);
                    if (inventory != null)
                    {
                        inventory.AddQuantity(transaction.Quantity); 
                        _donationRepository.UpdateWarehouseInventory(inventory);
                    }
                }
            }

            return true;
        }

        public async Task<bool> ConfirmDonationRejectedAsync(Guid donationId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var donation = await _donationRepository.GetDonationWithTrackingAsync(donationId, cancellationToken);
            if (donation == null) return false;

            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể xác nhận từ chối cho các đơn đang ở trạng thái Pending.");
            }

            donation.Reject(coordinatorId); 
            _donationRepository.UpdateDonation(donation);
            return true;
        }

        public async Task<List<Donation>> GetAllDonationsAsync(Guid coordinatorId, CancellationToken cancellationToken)
        {
            var warehouse = await _donationRepository.GetWarehouseByCoordinatorIdAsync(coordinatorId, cancellationToken);
            if (warehouse == null)
            {
                return new List<Donation>();
            }

            return await _donationRepository.GetAllDonationsByWarehouseIdAsync(warehouse.Id, cancellationToken);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle) => angle * Math.PI / 180;
    }
}