using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Commands;

public record VolunteerSkillInput(Guid SkillId, int Level);

public record SubmitVolunteerProfileCommand(
    Guid UserId,
    int ExperienceYears,
    string? CVUrl,
    List<VolunteerSkillInput> Skills
) : IRequest<VolunteerProfileDto?>;

public class SubmitVolunteerProfileCommandHandler
    : IRequestHandler<SubmitVolunteerProfileCommand, VolunteerProfileDto?>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVolunteerProfileCommandHandler(
        IVolunteerService volunteerService,
        IUnitOfWork unitOfWork)
    {
        _volunteerService = volunteerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VolunteerProfileDto?> Handle(
        SubmitVolunteerProfileCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var skills = request.Skills.Select(s => (s.SkillId, s.Level));

            var volunteer = await _volunteerService.CreateProfileAsync(
                request.UserId,
                request.ExperienceYears,
                request.CVUrl,
                skills,
                cancellationToken);

            if (volunteer == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return null;
            }

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

public class SubmitVolunteerProfileCommandValidator
    : AbstractValidator<SubmitVolunteerProfileCommand>
{
    public SubmitVolunteerProfileCommandValidator()
    {
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
                .GreaterThan(0)
                .WithMessage("Skill level must be greater than 0.");
        });
    }
}