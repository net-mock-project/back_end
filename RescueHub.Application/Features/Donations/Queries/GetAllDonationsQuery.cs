using MediatR;
using Mapster;
using RescueHub.Application.Contracts.Donation;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Donations.Queries
{
    public record GetAllDonationsQuery(Guid CoordinatorId) : IRequest<List<DonationDto>>;

    public class GetAllDonationsQueryHandler : IRequestHandler<GetAllDonationsQuery, List<DonationDto>>
    {
        private readonly IDonationService _donationService;

        public GetAllDonationsQueryHandler(IDonationService donationService)
        {
            _donationService = donationService;
        }

        public async Task<List<DonationDto>> Handle(GetAllDonationsQuery request, CancellationToken cancellationToken)
        {
            var donations = await _donationService.GetAllDonationsAsync(request.CoordinatorId, cancellationToken);
            return donations.Adapt<List<DonationDto>>();
        }
    }
}