using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Notifications;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Commands;

public record RejectVolunteerProfileCommand(
    Guid VolunteerId,
    Guid ApproverId,
    string? Reason = null
) : IRequest<VolunteerProfileDto?>;

public class RejectVolunteerProfileCommandHandler
    : IRequestHandler<RejectVolunteerProfileCommand, VolunteerProfileDto?>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public RejectVolunteerProfileCommandHandler(
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

    public async Task<VolunteerProfileDto?> Handle(
        RejectVolunteerProfileCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var volunteer = await _volunteerService.RejectProfileAsync(
                request.VolunteerId,
                request.ApproverId,
                cancellationToken);

            if (volunteer == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return null;
            }

            var auditLog = new AuditLog(
                userId: request.ApproverId,
                action: "REJECT",
                entityName: nameof(Volunteer),
                entityId: request.VolunteerId,
                oldValue: VolunteerApprovalStatus.Pending.ToString(),
                newValue: VolunteerApprovalStatus.Rejected.ToString());

            await _auditLogService.CreateAsync(auditLog, cancellationToken);

            var content = string.IsNullOrWhiteSpace(request.Reason)
                ? "Your volunteer application has been rejected."
                : $"Your volunteer application has been rejected. Reason: {request.Reason}";

            var notification = new Notification(
                userId: request.VolunteerId,
                title: "Volunteer application rejected",
                content: content,
                type: NotificationType.Volunteer);

            await _notificationService.CreateAsync(notification, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return volunteer.Adapt<VolunteerProfileDto>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public class RejectVolunteerProfileCommandValidator
    : AbstractValidator<RejectVolunteerProfileCommand>
    {
        public RejectVolunteerProfileCommandValidator()
        {
            RuleFor(x => x.VolunteerId)
                .NotEmpty()
                .WithMessage("Volunteer ID is required.");

            RuleFor(x => x.ApproverId)
                .NotEmpty()
                .WithMessage("Approver ID is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Reason))
                .WithMessage("Rejection reason must not exceed 500 characters.");
        }
    }
}