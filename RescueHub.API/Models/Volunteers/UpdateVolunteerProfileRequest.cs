namespace RescueHub.API.Models.Volunteers
{
    public class UpdateVolunteerProfileRequest
    {
        public int ExperienceYears { get; set; }
        public string? CVUrl { get; set; }
        public List<VolunteerSkillRequest> Skills { get; set; } = new();
    }
}