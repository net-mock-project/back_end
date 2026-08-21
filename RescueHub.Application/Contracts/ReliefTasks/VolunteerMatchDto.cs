namespace RescueHub.Application.Contracts.ReliefTasks;

public record VolunteerMatchDto(
    Guid VolunteerId,
    string FullName,
    string ProfileUrl,
    int DistanceScore,
    int SkillScore,
    int AvailabilityScore,
    int TotalScore,
    List<Guid> MatchedSkills
);
