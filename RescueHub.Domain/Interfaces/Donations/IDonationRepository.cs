using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Donations
{
    public interface IDonationRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken cancellationToken);
        Task<Supply?> GetSupplyByNameAsync(string supplyName, CancellationToken cancellationToken);
        Task AddSupplyAsync(Supply supply, CancellationToken cancellationToken);
        Task<WarehouseInventory?> GetWarehouseInventoryAsync(Guid warehouseId, Guid supplyId, CancellationToken cancellationToken);
        Task AddWarehouseInventoryAsync(WarehouseInventory warehouseInventory, CancellationToken cancellationToken);
        Task AddDonationAsync(Donation donation, CancellationToken cancellationToken);
        Task AddTransactionAsync(WarehouseTransaction transaction, CancellationToken cancellationToken);
        Task AddDonationTransactionAsync(DonationTransaction donationTransaction, CancellationToken cancellationToken);

        Task<Donation?> GetDonationByIdAndUserIdAsync(Guid donationId, Guid userId, CancellationToken cancellationToken);
        Task<List<Donation>> GetDonationsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Supply>> GetAllSuppliesAsync(CancellationToken cancellationToken);

        Task<WarehouseInventory?> GetWarehouseInventoryByIdAsync(Guid warehouseInventoryId, CancellationToken cancellationToken);
        Task<WarehouseTransaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken);
        Task<List<WarehouseTransaction>> GetTransactionsByDonationIdAsync(Guid donationId, CancellationToken cancellationToken);
        Task<Donation?> GetDonationByIdAsync(Guid donationId, CancellationToken cancellationToken);
        Task<Warehouse?> GetWarehouseByCoordinatorIdAsync(Guid coordinatorId, CancellationToken cancellationToken);
        Task<List<Donation>> GetAllDonationsByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken);

        // --- CÁC PHƯƠNG THỨC XỬ LÝ CẬP NHẬT ---
        Task<Donation?> GetDonationWithTrackingAsync(Guid donationId, CancellationToken cancellationToken);
        Task<Donation?> GetDonationWithTrackingByIdAndUserIdAsync(Guid donationId, Guid userId, CancellationToken cancellationToken);
        Task<WarehouseInventory?> GetWarehouseInventoryWithTrackingAsync(Guid warehouseInventoryId, CancellationToken cancellationToken);
        Task<WarehouseTransaction?> GetTransactionWithTrackingAsync(Guid transactionId, CancellationToken cancellationToken);

        void UpdateDonation(Donation donation);
        void RemoveDonationTransaction(DonationTransaction donationTransaction);
        void UpdateWarehouseInventory(WarehouseInventory inventory);
    }
}