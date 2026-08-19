namespace RescueHub.API.Models.Volunteers
{
    public class CoordinatorCreateVolunteerRequest
    {
        public Guid UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string? CVUrl { get; set; }
        public List<VolunteerSkillRequest> Skills { get; set; } = new();
    }
}