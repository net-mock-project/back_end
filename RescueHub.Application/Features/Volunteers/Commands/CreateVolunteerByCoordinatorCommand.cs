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

public record CreateVolunteerByCoordinatorCommand(
    Guid CoordinatorId,
    Guid TargetUserId,
    int ExperienceYears,
    string? CVUrl,
    List<VolunteerSkillInput> Skills
) : IRequest<VolunteerProfileDto?>;

public class CreateVolunteerByCoordinatorCommandHandler
    : IRequestHandler<CreateVolunteerByCoordinatorCommand, VolunteerProfileDto?>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVolunteerByCoordinatorCommandHandler(
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
        CreateVolunteerByCoordinatorCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var skills = request.Skills.Select(s => (s.SkillId, s.Level));

            var volunteer = await _volunteerService.CreateByCoordinatorAsync(
                request.CoordinatorId,
                request.TargetUserId,
                request.ExperienceYears,
                request.CVUrl,
                skills,
                cancellationToken);

            if (volunteer == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return null;
            }

            var auditLog = new AuditLog(
                userId: request.CoordinatorId,
                action: "CREATE_VOLUNTEER",
                entityName: nameof(Volunteer),
                entityId: request.TargetUserId,
                oldValue: null,
                newValue: VolunteerApprovalStatus.Approved.ToString());
            await _auditLogService.CreateAsync(auditLog, cancellationToken);

            var notification = new Notification(
                userId: request.TargetUserId,
                title: "Assigned as Volunteer",
                content: "You have been onboarded as an official volunteer by your regional coordinator.",
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
}

public class CreateVolunteerByCoordinatorCommandValidator
    : AbstractValidator<CreateVolunteerByCoordinatorCommand>
{
    public CreateVolunteerByCoordinatorCommandValidator()
    {
        RuleFor(x => x.CoordinatorId)
            .NotEmpty()
            .WithMessage("Coordinator ID is required.");

        RuleFor(x => x.TargetUserId)
            .NotEmpty()
            .WithMessage("Target User ID is required.");

        RuleFor(x => x.ExperienceYears)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Experience years cannot be negative.");

        RuleFor(x => x.CVUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.CVUrl))
            .WithMessage("CV URL must not exceed 500 characters.");

        RuleForEach(x => x.Skills).ChildRules(skill =>
        {
            skill.RuleFor(s => s.SkillId)
                .NotEmpty()
                .WithMessage("Skill ID is required.");

            skill.RuleFor(s => s.Level)
                .InclusiveBetween(1, 5)
                .WithMessage("Skill level must be between 1 and 5.");
        });
    }
}