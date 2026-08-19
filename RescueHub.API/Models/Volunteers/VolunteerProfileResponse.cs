namespace RescueHub.API.Models.Volunteers
{
    public class VolunteerProfileResponse
    {
        public Guid Id { get; set; }

        public int ExperienceYears { get; set; }

        public string? CVUrl { get; set; }

        public string ApprovalStatus { get; set; } = null!;

        public DateTime? ApprovedAt { get; set; }
    }
}