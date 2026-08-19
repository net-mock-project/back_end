using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Commands;

public record UpdateVolunteerByCoordinatorCommand(
    Guid CoordinatorId,
    Guid TargetVolunteerId,
    int ExperienceYears,
    string? CVUrl,
    List<VolunteerSkillInput> Skills
) : IRequest<VolunteerProfileDto?>;

public class UpdateVolunteerByCoordinatorCommandHandler
    : IRequestHandler<UpdateVolunteerByCoordinatorCommand, VolunteerProfileDto?>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVolunteerByCoordinatorCommandHandler(
        IVolunteerService volunteerService,
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork)
    {
        _volunteerService = volunteerService;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VolunteerProfileDto?> Handle(
        UpdateVolunteerByCoordinatorCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var skills = request.Skills.Select(s => (s.SkillId, s.Level));

            var volunteer = await _volunteerService.UpdateByCoordinatorAsync(
                request.CoordinatorId,
                request.TargetVolunteerId,
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
                action: "UPDATE_VOLUNTEER",
                entityName: nameof(Volunteer),
                entityId: request.TargetVolunteerId);
            await _auditLogService.CreateAsync(auditLog, cancellationToken);

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

public class UpdateVolunteerByCoordinatorCommandValidator
    : AbstractValidator<UpdateVolunteerByCoordinatorCommand>
{
    public UpdateVolunteerByCoordinatorCommandValidator()
    {
        RuleFor(x => x.CoordinatorId)
            .NotEmpty()
            .WithMessage("Coordinator ID is required.");

        RuleFor(x => x.TargetVolunteerId)
            .NotEmpty()
            .WithMessage("Volunteer ID is required.");

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