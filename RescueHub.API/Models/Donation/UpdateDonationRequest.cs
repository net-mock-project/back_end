using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models.Donation
{
    public class UpdateDonationRequest 
    {
        public string? SupplyName { get; set; }

        public int? Quantity { get; set; }

        public string? Unit { get; set; }

        public DateTime? DonationDate { get; set; }

    }
}