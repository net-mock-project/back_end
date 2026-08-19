using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Queries
{
    // Query lấy thông tin hồ sơ Volunteer của User
    public record GetVolunteerProfileQuery(
        Guid VolunteerId
    ) : IRequest<VolunteerProfileDto?>;

    public class GetVolunteerProfileQueryHandler
        : IRequestHandler<GetVolunteerProfileQuery, VolunteerProfileDto?>
    {
        private readonly IVolunteerService _volunteerService;

        public GetVolunteerProfileQueryHandler(
            IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<VolunteerProfileDto?> Handle(
            GetVolunteerProfileQuery request,
            CancellationToken cancellationToken)
        {
            var volunteer = await _volunteerService.GetProfileAsync(
                request.VolunteerId,
                cancellationToken);

            return volunteer?.Adapt<VolunteerProfileDto>();
        }
    }
}