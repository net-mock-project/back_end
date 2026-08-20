using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.TaskAssignments;

public record TaskAssignmentDto(
    Guid Id,
    Guid TaskId,
    Guid VolunteerId,
    Guid AssignedBy,
    TaskAssignmentSource Source,
    TaskAssignmentStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
