using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Queries;

public record GetApprovedVolunteerProfilesQuery(
    Guid CoordinatorId,
    QueryCriteria Criteria
) : IRequest<PaginationResponse<VolunteerProfileDto>>;

public class GetApprovedVolunteerProfilesQueryHandler
    : IRequestHandler<GetApprovedVolunteerProfilesQuery, PaginationResponse<VolunteerProfileDto>>
{
    private readonly IVolunteerService _volunteerService;

    public GetApprovedVolunteerProfilesQueryHandler(IVolunteerService volunteerService)
    {
        _volunteerService = volunteerService;
    }

    public async Task<PaginationResponse<VolunteerProfileDto>> Handle(
        GetApprovedVolunteerProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _volunteerService.GetApprovedProfilesAsync(
            request.CoordinatorId,
            request.Criteria,
            cancellationToken);

        var items = result.Items
            .Select(x => x.Adapt<VolunteerProfileDto>())
            .ToList();

        return new PaginationResponse<VolunteerProfileDto>(
            items,
            result.TotalCount,
            request.Criteria.PageNumber,
            request.Criteria.PageSize);
    }
}