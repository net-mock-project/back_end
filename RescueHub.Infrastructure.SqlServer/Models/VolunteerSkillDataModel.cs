namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class VolunteerSkillDataModel
    {
        public Guid VolunteerId { get; set; }

        public Guid SkillId { get; set; }

        public int Level { get; set; }

        public VolunteerDataModel Volunteer { get; set; } = null!;

        public SkillDataModel Skill { get; set; } = null!;
    }
}
