using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Notifications;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Commands;

public record DeleteVolunteerByCoordinatorCommand(
    Guid CoordinatorId,
    Guid TargetVolunteerId,
    string? Reason = null
) : IRequest<bool>;

public class DeleteVolunteerByCoordinatorCommandHandler
    : IRequestHandler<DeleteVolunteerByCoordinatorCommand, bool>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVolunteerByCoordinatorCommandHandler(
        IVolunteerService volunteerService,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _volunteerService = volunteerService;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        DeleteVolunteerByCoordinatorCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var isSuccess = await _volunteerService.DeleteByCoordinatorAsync(
                request.CoordinatorId,
                request.TargetVolunteerId,
                cancellationToken);

            if (!isSuccess)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return false;
            }

            var auditLog = new AuditLog(
                userId: request.CoordinatorId,
                action: "REVOKE_VOLUNTEER",
                entityName: nameof(Volunteer),
                entityId: request.TargetVolunteerId,
                oldValue: "Approved",
                newValue: "Deleted");
            await _auditLogService.CreateAsync(auditLog, cancellationToken);

            var content = string.IsNullOrWhiteSpace(request.Reason)
                ? "Your volunteer membership has been revoked by the regional coordinator."
                : $"Your volunteer membership has been revoked. Reason: {request.Reason}";

            var notification = new Notification(
                userId: request.TargetVolunteerId,
                title: "Volunteer Membership Revoked",
                content: content,
                type: NotificationType.Volunteer);
            await _notificationService.CreateAsync(notification, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

public class DeleteVolunteerByCoordinatorCommandValidator
    : AbstractValidator<DeleteVolunteerByCoordinatorCommand>
{
    public DeleteVolunteerByCoordinatorCommandValidator()
    {
        RuleFor(x => x.CoordinatorId)
            .NotEmpty()
            .WithMessage("Coordinator ID is required.");

        RuleFor(x => x.TargetVolunteerId)
            .NotEmpty()
            .WithMessage("Volunteer ID is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Revocation reason must not exceed 500 characters.");
    }
}