namespace RescueHub.API.Models.ReliefRequests
{
    public class UpdateReliefRequestApiRequest
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ReliefImageUrl { get; set; }
        public string? RequestedResource { get; set; }
        public int UrgencyLevel { get; set; }
        public int EstimatedAffectedPeople { get; set; }
        public decimal? EstimatedAffectedRadiusKm { get; set; }
    }
}
