using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models.Donation
{
	// Dữ liệu Client gửi lên để tạo donation
	public class CreateDonationRequest
	{
		public string SupplyName { get; set; } = string.Empty;

		public int Quantity { get; set; }
		
		public string Unit { get; set; } = string.Empty;

		public DateTime DonationDate { get; set; }
	}
}