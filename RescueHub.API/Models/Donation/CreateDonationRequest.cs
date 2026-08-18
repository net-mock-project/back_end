
namespace RescueHub.API.Models.Donation
{
	// Dữ liệu Client gửi lên để tạo donation
	public class CreateDonationRequest
	{
        public List<DonationItemRequest> Items { get; set; } = new();

        public DateTime DonationDate { get; set; }
	}
}