using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.ReliefRequests
{
    public class ReliefRequestResponse
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid? CoordinatorId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ReliefImageUrl { get; set; }
        public string? RequestedResource { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int UrgencyLevel { get; set; }
        public int EstimatedAffectedPeople { get; set; }
        public decimal? EstimatedAffectedRadiusKm { get; set; }
        public ReliefRequestStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
