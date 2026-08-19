using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Interfaces.Volunteers;
using Mapster;

namespace RescueHub.Application.Features.Volunteers.Commands;

public record RejectVolunteerProfileCommand(
    Guid VolunteerId,
    Guid ApproverId
) : IRequest<VolunteerProfileDto?>;

public class RejectVolunteerProfileCommandHandler
    : IRequestHandler<RejectVolunteerProfileCommand, VolunteerProfileDto?>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IUnitOfWork _unitOfWork;

    public RejectVolunteerProfileCommandHandler(
        IVolunteerService volunteerService,
        IUnitOfWork unitOfWork)
    {
        _volunteerService = volunteerService;
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