namespace RescueHub.Application.Contracts.Volunteers
{
    public class VolunteerProfileDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Province { get; set; }
        public int ExperienceYears { get; set; }
        public string? CVUrl { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public DateTime? ApprovedAt { get; set; }
        public List<VolunteerSkillDto> Skills { get; set; } = new();
    }
}