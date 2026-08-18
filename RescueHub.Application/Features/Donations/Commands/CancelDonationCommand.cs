using MediatR;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Donations.Commands
{
    public record CancelDonationCommand(Guid UserId, Guid DonationId) : IRequest<bool>;

    public class CancelDonationCommandHandler : IRequestHandler<CancelDonationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDonationService _donationService;

        public CancelDonationCommandHandler(IUnitOfWork unitOfWork, IDonationService donationService)
        {
            _unitOfWork = unitOfWork;
            _donationService = donationService;
        }

        public async Task<bool> Handle(CancelDonationCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var success = await _donationService.CancelDonationAsync(request.UserId, request.DonationId, cancellationToken);
                if (!success)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return false;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}