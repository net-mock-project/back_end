using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.ReliefTasks;

public record ReliefTaskDto(
    Guid Id,
    Guid RequestId,
    string Title,
    string Description,
    int RequiredVolunteers,
    int Priority,
    double? Latitude,
    double? Longitude,
    ReliefTaskStatus Status,
    List<Guid> TaskSkills,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
