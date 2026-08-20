namespace RescueHub.Domain.Entities
{
    public class VolunteerSkill
    {
        public Guid VolunteerId { get; private set; }
        public Guid SkillId { get; private set; }
        public int Level { get; private set; }
        public string? SkillName { get; private set; }

        public VolunteerSkill(Guid volunteerId, Guid skillId, int level, string? skillName = null)
        {
            if (skillId == Guid.Empty)
                throw new ArgumentException("Skill ID cannot be empty.", nameof(skillId));

            if (level <= 0)
                throw new ArgumentException("Skill level must be greater than 0.", nameof(level));

            VolunteerId = volunteerId;
            SkillId = skillId;
            Level = level;
            SkillName = skillName;
        }
    }
}