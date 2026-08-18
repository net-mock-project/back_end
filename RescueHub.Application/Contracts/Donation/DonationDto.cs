using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.Donation
{
    public class DonationDto
    {
        public Guid DonationId { get; set; }
        public string DonatorName { get; set; } = null!;
        public string DonatorPhone { get; set; } = null!;
        public List<DonationItemRequest> Items { get; set; } = new();
        public string WarehouseName { get; set; } = null!;
        public DateTime DonationDate { get; set; }
        public DonationStatus Status { get; set; } = DonationStatus.Pending;
    }

    public class DonationItemRequest
    {
        public string SupplyName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string Unit { get; set; } = string.Empty;
    }

}