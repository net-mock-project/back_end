using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.VolunteerEngagements;

public record VolunteerEngagementDto(
    Guid Id,
    Guid VolunteerId,
    Guid RequestId,
    VolunteerEngagementStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
