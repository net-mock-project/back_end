namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class TaskSkillDataModel
    {
        public Guid TaskId { get; set; }

        public Guid SkillId { get; set; }

        public ReliefTaskDataModel Task { get; set; } = null!;

        public SkillDataModel Skill { get; set; } = null!;
    }
}
