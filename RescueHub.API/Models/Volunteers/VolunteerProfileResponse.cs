namespace RescueHub.API.Models.Volunteers
{
    public class VolunteerSkillResponse
    {
        public Guid SkillId { get; set; }
        public string? SkillName { get; set; }
        public int Level { get; set; }
    }

    public class VolunteerProfileResponse
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
        public List<VolunteerSkillResponse> Skills { get; set; } = new();
    }
}