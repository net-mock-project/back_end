using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Queries
{
    public record GetVolunteerProfileByIdQuery(
        Guid VolunteerId
    ) : IRequest<VolunteerProfileDto?>;

    public class GetVolunteerProfileByIdQueryHandler
        : IRequestHandler<GetVolunteerProfileByIdQuery, VolunteerProfileDto?>
    {
        private readonly IVolunteerService _volunteerService;

        public GetVolunteerProfileByIdQueryHandler(
            IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<VolunteerProfileDto?> Handle(
            GetVolunteerProfileByIdQuery request,
            CancellationToken cancellationToken)
        {
            var volunteer = await _volunteerService.GetProfileAsync(
                request.VolunteerId,
                cancellationToken);

            return volunteer?.Adapt<VolunteerProfileDto>();
        }
    }
}