using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Donations
{
    public interface IDonationService
    {
        // User
        Task<Donation> CreateDonationAsync(
            Guid DonatorId,
            string SupplyName,
            int Quantity,
            string Unit,
            DateTime DonationDate, CancellationToken cancellationToken);
        Task<Donation?> UpdateDonationAsync(
            Guid UserId,
            Guid DonationId,         
            string? SupplyName,
            int? Quantity,
            string? Unit,
            DateTime? DonationDate,
            CancellationToken cancellationToken);
        Task<bool> CancelDonationAsync(Guid userId, Guid donationId, CancellationToken cancellationToken);
        Task<List<Donation>> GetDonationsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        // Coordinator
        Task<bool> ConfirmDonationReceivedAsync(Guid donationId, Guid coordinatorId, CancellationToken cancellationToken);
        Task<bool> ConfirmDonationRejectedAsync(Guid donationId, Guid coordinatorId, CancellationToken cancellationToken);
        Task<List<Donation>> GetAllDonationsAsync(Guid coordinatorId, CancellationToken cancellationToken);
    }
}