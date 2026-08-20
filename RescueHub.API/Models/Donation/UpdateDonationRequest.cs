
namespace RescueHub.API.Models.Donation
{
    public class UpdateDonationRequest 
    {
        public List<DonationItemRequest?> Items { get; set; } = new();

        public DateTime? DonationDate { get; set; }
    }
}