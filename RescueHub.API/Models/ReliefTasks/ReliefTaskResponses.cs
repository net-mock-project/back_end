using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.ReliefTasks;

public class ReliefTaskResponse
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int RequiredVolunteers { get; set; }
    public int Priority { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public ReliefTaskStatus Status { get; set; }
    public List<Guid> TaskSkills { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TaskAssignmentResponse
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid VolunteerId { get; set; }
    public Guid AssignedBy { get; set; }
    public TaskAssignmentSource Source { get; set; }
    public TaskAssignmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
