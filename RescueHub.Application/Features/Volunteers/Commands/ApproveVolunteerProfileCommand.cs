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

public record ApproveVolunteerProfileCommand(
    Guid VolunteerId,
    Guid ApproverId
) : IRequest<VolunteerProfileDto?>;

public class ApproveVolunteerProfileCommandHandler
    : IRequestHandler<ApproveVolunteerProfileCommand, VolunteerProfileDto?>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveVolunteerProfileCommandHandler(
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
        ApproveVolunteerProfileCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var volunteer = await _volunteerService.ApproveProfileAsync(
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
                action: "APPROVE",
                entityName: nameof(Volunteer),
                entityId: request.VolunteerId,
                oldValue: VolunteerApprovalStatus.Pending.ToString(),
                newValue: VolunteerApprovalStatus.Approved.ToString());

            await _auditLogService.CreateAsync(auditLog, cancellationToken);

            var notification = new Notification(
                userId: request.VolunteerId,
                title: "Volunteer application approved",
                content: "Congratulations! Your volunteer application has been approved. You can now accept relief tasks.",
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

    public class ApproveVolunteerProfileCommandValidator
        : AbstractValidator<ApproveVolunteerProfileCommand>
    {
        public ApproveVolunteerProfileCommandValidator()
        {
            RuleFor(x => x.VolunteerId)
                .NotEmpty()
                .WithMessage("Volunteer ID is required.");

            RuleFor(x => x.ApproverId)
                .NotEmpty()
                .WithMessage("Approver ID is required.");
        }
    }
}