using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Donation;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Donations.Queries
{
    public record GetMySupplyQuery() : IRequest<List<string>>;

    public class GetMySupplyQueryHandler : IRequestHandler<GetMySupplyQuery, List<string>>
    {
        private readonly IDonationService _donationService;

        public GetMySupplyQueryHandler(IDonationService donationService)
        {
            _donationService = donationService;
        }

        public async Task<List<string>> Handle(GetMySupplyQuery request, CancellationToken cancellationToken)
        {
           
            var supplies = await _donationService.GetSuppliesNameAsync(cancellationToken);

            return supplies;
        }
    }
}