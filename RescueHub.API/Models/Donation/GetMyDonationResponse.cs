using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Donation
{
    // dữ liệu request để lấy danh sách các Donation của user
    public class GetMyDonationResponse
    {
        public Guid DonationId { get; set; }
        public string DonatorName { get; set; } = null!;
        public string DonatorPhone { get; set; } = null!;
        public string SupplyName { get; set; } = null!;
        public int Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public DateTime DonationDate { get; set; }
        public DonationStatus Status { get; set; } = DonationStatus.Pending;
    }
}