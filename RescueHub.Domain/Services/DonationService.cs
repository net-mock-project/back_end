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
            List<(string SupplyName, int Quantity, string Unit)> Items,
            DateTime DonationDate,
            CancellationToken cancellationToken)
        {
            if (Items == null || !Items.Any())
            {
                throw new Exception("At least one donation item is required.");
            }

            // 1. Lấy thông tin User để lấy tọa độ
            var user = await _donationRepository.GetUserByIdAsync(DonatorId, cancellationToken);
            if (user == null || user.Location == null)
            {
                throw new Exception($"User '{DonatorId}' not found or location is not set.");
            }

            // 2. Lấy danh sách kho và tìm kho gần nhất
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

            // 3. Tạo Donation chính
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

            // 4. Duyệt qua từng item để tạo Supply, Inventory, Transaction và Link
            foreach (var item in Items)
            {
                // Tìm hoặc tạo Supply
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

                // Kiểm tra hoặc tạo WarehouseInventory trong kho gần nhất
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

                // Tạo WarehouseTransaction
                var transactionId = Guid.NewGuid();
                var transaction = new WarehouseTransaction
                {
                    Id = transactionId,
                    WarehouseInventoryId = warehouseInventoryId,
                    Quantity = item.Quantity,
                    TransactionType = WarehouseTransactionType.Import,
                    Status = WarehouseTransactionStatus.Pending,
                    CreatedBy = DonatorId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                };
                await _donationRepository.AddTransactionAsync(transaction, cancellationToken);

                // Tạo bảng trung gian DonationTransaction
                var donationTransaction = new DonationTransaction(donationId, transactionId);
                await _donationRepository.AddDonationTransactionAsync(donationTransaction, cancellationToken);
            }

            return donation;
        }

        public async Task<Donation?> UpdateDonationAsync(
            Guid userId,
            Guid donationId,
            List<(string SupplyName, int Quantity, string Unit)> Items,
            DateTime? donationDate,
            CancellationToken cancellationToken)
        {
            // 1. Lấy đơn quyên góp kèm theo các giao dịch và thông tin kho/vật phẩm cũ
            var donation = await _donationRepository.GetDonationByIdAndUserIdAsync(donationId, userId, cancellationToken);
            if (donation == null) return null;

            // 2. Kiểm tra trạng thái
            if (donation.Status != DonationStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể chỉnh sửa đơn quyên góp khi đang ở trạng thái chờ xử lý (Pending).");
            }

            // 3. Cập nhật ngày quyên góp nếu có
            if (donationDate.HasValue)
            {
                donation.DonationDate = donationDate.Value;
            }

            // 4. Nếu có truyền danh sách Items mới, tiến hành đồng bộ
            if (Items != null && Items.Any())
            {
                // Lấy kho hiện tại của đơn (dựa vào giao dịch cũ đầu tiên để biết đơn này đang gắn với kho nào)
                var firstTransaction = donation.DonationTransactions.FirstOrDefault()?.WarehouseTransactions;
                if (firstTransaction?.WarehouseInventories == null)
                {
                    throw new Exception("Không tìm thấy thông tin kho liên kết với đơn quyên góp này.");
                }
                Guid warehouseId = firstTransaction.WarehouseInventories.WarehouseId;

                // Xóa các transaction và liên kết cũ đi để tạo lại danh sách m
                // Lấy danh sách transaction cũ để xử lý xóa/thay thế
                foreach (var dt in donation.DonationTransactions.ToList())
                {
                    
                    // clear danh sách DonationTransactions
                    donation.DonationTransactions.Remove(dt);
                }

                // Tạo lại các Transaction mới tương ứng với danh sách Items mới
                foreach (var item in Items)
                {
                    // 4.1. Tìm hoặc tạo Supply mới nếu chưa có
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

                    // 4.2. Kiểm tra hoặc tạo WarehouseInventory trong kho tương ứng
                    var warehouseInventory = await _donationRepository.GetWarehouseInventoryAsync(warehouseId, supplyId, cancellationToken);
                    Guid warehouseInventoryId;
                    if (warehouseInventory == null)
                    {
                        warehouseInventoryId = Guid.NewGuid();
                        warehouseInventory = new WarehouseInventory(
                            id: warehouseInventoryId,
                            warehouseId: warehouseId,
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

                    // 4.3. Tạo WarehouseTransaction mới
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
                        UpdatedAt = null
                    };
                    await _donationRepository.AddTransactionAsync(newTransaction, cancellationToken);

                    // 4.4. Tạo liên kết DonationTransaction mới
                    var donationTransaction = new DonationTransaction(donation.Id, transactionId);
                    await _donationRepository.AddDonationTransactionAsync(donationTransaction, cancellationToken);

                    // Thêm vào navigation property để trả về kết quả ngay nếu cần
                    donation.DonationTransactions.Add(donationTransaction);
                }
            }

            // 5. Đánh dấu donation đã được cập nhật thời gian
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

            donationEntity.UpdateStatus(DonationStatus.Completed, coordinatorId);

            // Cộng dồn số lượng vào kho cho TẤT CẢ các vật phẩm trong đơn
            foreach (var donationTransaction in donationEntity.DonationTransactions)
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