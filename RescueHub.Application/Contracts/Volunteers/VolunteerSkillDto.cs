namespace RescueHub.Application.Contracts.Volunteers
{
    public class VolunteerSkillDto
    {
        public Guid SkillId { get; set; }
        public string? SkillName { get; set; }
        public int Level { get; set; }
    }
}