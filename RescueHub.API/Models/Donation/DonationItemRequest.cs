namespace RescueHub.API.Models.Donation
{
    public class DonationItemRequest
    {
        public string SupplyName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}