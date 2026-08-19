namespace RescueHub.API.Models.Volunteers
{
    public class SubmitVolunteerProfileRequest
    {
        public int ExperienceYears { get; set; }

        public string? CVUrl { get; set; }
    }
}