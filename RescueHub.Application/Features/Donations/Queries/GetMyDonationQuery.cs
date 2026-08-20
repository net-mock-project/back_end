using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Donation;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Donations.Queries
{
    public record GetMyDonationQuery(Guid UserId) : IRequest<List<DonationDto>>;

    public class GetMyDonationQueryHandler : IRequestHandler<GetMyDonationQuery, List<DonationDto>>
    {
        private readonly IDonationService _donationService;

        public GetMyDonationQueryHandler(IDonationService donationService)
        {
            _donationService = donationService;
        }

        public async Task<List<DonationDto>> Handle(GetMyDonationQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy danh sách Domain Entity từ Service
            var donations = await _donationService.GetDonationsByUserIdAsync(request.UserId, cancellationToken);

            // 2. Dùng Mapster để biến đổi sang List<DonationDto> bằng cấu hình bạn đã viết ở ApplicationMappingRegister
            return donations.Adapt<List<DonationDto>>();
        }
    }
}