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
            Guid DonatorId,
            string SupplyName,
            int Quantity,
            string Unit,
            DateTime DonationDate, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin User để lấy tọa độ (Latitude, Longitude)
            var user = await _donationRepository.GetUserByIdAsync(DonatorId, cancellationToken);
            if (user == null || user.Location == null)
            {
                throw new Exception($"User '{DonatorId}' not found or location is not set.");
            }

            // 2. Lấy danh sách kho và tính toán kho gần nhất bằng công thức Haversine
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

            // 3. Tìm hoặc tạo mới Supply
            var supply = await _donationRepository.GetSupplyByNameAsync(SupplyName, cancellationToken);
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
                    name: SupplyName,
                    category: "General",
                    unit: Unit,
                    minimumStock: 10,
                    createdAt: DateTime.UtcNow,
                    updatedAt: null,
                    deletedAt: null
                );
                await _donationRepository.AddSupplyAsync(supply, cancellationToken);
            }

            // 4. Kiểm tra hoặc tạo WarehouseInventory
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

            // 5. Tạo Donation
            var donationId = Guid.NewGuid();
            var donation = new Donation(
                id: donationId,
                donatorId: DonatorId,
                status: DonationStatus.Pending,
                donationDate: DonationDate,
                approvedBy: null,
                approvedAt: null,
                remark: null,
                createdAt: DateTime.UtcNow,
                updatedAt: null
            );
            await _donationRepository.AddDonationAsync(donation, cancellationToken);

            // 6. Tạo WarehouseTransaction
            var transactionId = Guid.NewGuid();
            var transaction = new WarehouseTransaction
            {
                Id = transactionId,
                WarehouseInventoryId = warehouseInventoryId,
                Quantity = Quantity,
                TransactionType = WarehouseTransactionType.Import,
                Status = WarehouseTransactionStatus.Pending,
                CreatedBy = DonatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            await _donationRepository.AddTransactionAsync(transaction, cancellationToken);

            // 7. Tạo bảng trung gian DonationTransaction
            var donationTransaction = new DonationTransaction(donationId, transactionId);
            await _donationRepository.AddDonationTransactionAsync(donationTransaction, cancellationToken);

            // 8. Trả về Domain Entity Donation vừa tạo
            return donation;
        }

        public async Task<Donation?> UpdateDonationAsync(
            Guid userId,
            Guid donationId,
            string? supplyName,
            int? quantity,
            string? unit,
            DateTime? donationDate,
            CancellationToken cancellationToken)
        {
            // 1. Lấy donation theo ID và UserId (dùng đúng tên tham số truyền vào)
            var donation = await _donationRepository.GetDonationByIdAndUserIdAsync(donationId, userId, cancellationToken);
            if (donation == null) return null;

            // 2. Kiểm tra trạng thái
            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể chỉnh sửa đơn quyên góp khi đang ở trạng thái chờ xử lý (Pending).");
            }

            if (donationDate.HasValue)
            {
                donation.DonationDate = donationDate.Value;
            }

            // 3. Nếu có cập nhật tên vật phẩm hoặc đơn vị tính (Supply/Unit)
            if (!string.IsNullOrWhiteSpace(supplyName))
            {
                var supply = await _donationRepository.GetSupplyByNameAsync(supplyName, cancellationToken);
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
                        name: supplyName,
                        category: "General",
                        unit: unit ?? "pcs", // Dùng unit mới hoặc mặc định nếu null
                        minimumStock: 10,
                        createdAt: DateTime.UtcNow,
                        updatedAt: null,
                        deletedAt: null
                    );
                    await _donationRepository.AddSupplyAsync(supply, cancellationToken);
                }

                // Lấy transaction hiện tại của đơn này để cập nhật WarehouseInventoryId nếu đổi Supply
                var warehouseTransaction = await _donationRepository.GetTransactionByDonationIdAsync(donationId, cancellationToken);
                if (warehouseTransaction != null)
                {
                    // Lấy kho hiện tại từ transaction cũ
                    var oldInventory = await _donationRepository.GetWarehouseInventoryByIdAsync(warehouseTransaction.WarehouseInventoryId, cancellationToken);
                    if (oldInventory != null)
                    {
                        // Kiểm tra xem trong kho đó đã có inventory cho supply mới chưa
                        var newInventory = await _donationRepository.GetWarehouseInventoryAsync(oldInventory.WarehouseId, supplyId, cancellationToken);
                        Guid newInventoryId;

                        if (newInventory == null)
                        {
                            newInventoryId = Guid.NewGuid();
                            var inventoryToAdd = new WarehouseInventory(
                                id: newInventoryId,
                                warehouseId: oldInventory.WarehouseId,
                                supplyId: supplyId,
                                quantity: 0,
                                createdAt: DateTime.UtcNow,
                                updatedAt: null,
                                deletedAt: null
                            );
                            await _donationRepository.AddWarehouseInventoryAsync(inventoryToAdd, cancellationToken);
                        }
                        else
                        {
                            newInventoryId = newInventory.Id;
                        }

                        // Cập nhật lại WarehouseInventoryId trong Transaction
                        warehouseTransaction.WarehouseInventoryId = newInventoryId;
                    }
                }
            }

            // 4. Nếu có cập nhật số lượng (Quantity)
            if (quantity.HasValue && quantity.Value > 0)
            {
                var warehouseTransaction = await _donationRepository.GetTransactionByDonationIdAsync(donationId, cancellationToken);
                if (warehouseTransaction != null)
                {
                    warehouseTransaction.Quantity = quantity.Value;
                    warehouseTransaction.UpdatedAt = DateTime.UtcNow;
                }
            }

            // 5. Cập nhật thời gian chỉnh sửa cho Donation
            donation.UpdateDonation();

            return donation;
        }

        public async Task<bool> CancelDonationAsync(Guid userId, Guid donationId, CancellationToken cancellationToken)
        {
            var donation = await _donationRepository.GetDonationByIdAndUserIdAsync(donationId, userId, cancellationToken);
            if (donation == null) return false;

            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Không thể hủy đơn quyên góp đã được duyệt hoặc đã hoàn thành.");
            }

            donation.UpdateStatus(DonationStatus.Cancelled, userId);
            return true;
        }

        public async Task<List<Donation>> GetDonationsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _donationRepository.GetDonationsByUserIdAsync(userId, cancellationToken);
        }

        public async Task<bool> ConfirmDonationReceivedAsync(Guid donationId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var donationEntity = await _donationRepository.GetDonationByIdAsync(donationId, cancellationToken);
            if (donationEntity == null) return false;

            if (donationEntity.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể xác nhận nhận đồ cho các đơn đang ở trạng thái Pending.");
            }

            // 1. Cập nhật trạng thái đơn thành Completed
            donationEntity.UpdateStatus(DonationStatus.Completed, coordinatorId);

            // 2. Cộng dồn số lượng vào kho tương ứng thông qua DonationTransaction -> WarehouseTransaction
            var donationTransaction = donationEntity.DonationTransactions.FirstOrDefault();
            if (donationTransaction != null)
            {
                var transaction = await _donationRepository.GetTransactionByIdAsync(donationTransaction.TransactionId, cancellationToken);
                if (transaction != null)
                {
                    var warehouseInventory = await _donationRepository.GetWarehouseInventoryByIdAsync(transaction.WarehouseInventoryId, cancellationToken);
                    if (warehouseInventory != null)
                    {
                        int newQuantity = warehouseInventory.Quantity + transaction.Quantity;
                        warehouseInventory.UpdateQuantity(newQuantity);
                    }
                }
            }

            return true;
        }

        public async Task<bool> ConfirmDonationRejectedAsync(Guid donationId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var donationEntity = await _donationRepository.GetDonationByIdAsync(donationId, cancellationToken);
            if (donationEntity == null) return false;
            if (donationEntity.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể xác nhận từ chối cho các đơn đang ở trạng thái Pending.");
            }
            // Cập nhật trạng thái đơn thành Rejected
            donationEntity.UpdateStatus(DonationStatus.Rejected, coordinatorId);
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
            const double R = 6371; // Bán kính trái đất tính bằng km
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