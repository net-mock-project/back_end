namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class SkillDataModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<VolunteerSkillDataModel> VolunteerSkills { get; set; }
            = new List<VolunteerSkillDataModel>();

        public ICollection<TaskSkillDataModel> TaskSkills { get; set; }
            = new List<TaskSkillDataModel>();
    }
}
