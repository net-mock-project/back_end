namespace RescueHub.Application.Contracts.Volunteers
{
    public class VolunteerProfileDto
    {
        public Guid Id { get; set; }

        public int ExperienceYears { get; set; }

        public string? CVUrl { get; set; }

        public string ApprovalStatus { get; set; } = null!;

        public DateTime? ApprovedAt { get; set; }
    }
}