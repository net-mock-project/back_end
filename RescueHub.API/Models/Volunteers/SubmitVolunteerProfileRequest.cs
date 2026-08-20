namespace RescueHub.API.Models.Volunteers
{
    public class VolunteerSkillRequest
    {
        public Guid SkillId { get; set; }
        public int Level { get; set; }
    }

    public class SubmitVolunteerProfileRequest
    {
        public int ExperienceYears { get; set; }
        public string? CVUrl { get; set; }
        public List<VolunteerSkillRequest> Skills { get; set; } = new();
    }
}