using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Commands;

// Command đăng ký hồ sơ Volunteer
public record SubmitVolunteerProfileCommand(
    Guid UserId,
    int ExperienceYears,
    string? CVUrl
) : IRequest<VolunteerProfileDto?>;

// Handler xử lý đăng ký hồ sơ Volunteer
public class SubmitVolunteerProfileCommandHandler
    : IRequestHandler<
        SubmitVolunteerProfileCommand,
        VolunteerProfileDto?>
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
        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            var volunteer =
                await _volunteerService.CreateProfileAsync(
                    request.UserId,
                    request.ExperienceYears,
                    request.CVUrl,
                    cancellationToken);

            if (volunteer == null)
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                return null;
            }

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await _unitOfWork.CommitAsync(
                cancellationToken);

            return volunteer.Adapt<VolunteerProfileDto>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(
                cancellationToken);

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
            .WithMessage(
                "Experience years cannot be negative.");

        RuleFor(x => x.CVUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.CVUrl))
            .WithMessage(
                "CV URL must not exceed 500 characters.");
    }
}