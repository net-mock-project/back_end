using MediatR;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Donations.Commands
{
    public record ConfirmDonationReceivedCommand(Guid DonationId, Guid CoordinatorId) : IRequest<bool>;

    public class ConfirmDonationReceivedCommandHandler : IRequestHandler<ConfirmDonationReceivedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDonationService _donationService;

        public ConfirmDonationReceivedCommandHandler(IUnitOfWork unitOfWork, IDonationService donationService)
        {
            _unitOfWork = unitOfWork;
            _donationService = donationService;
        }

        public async Task<bool> Handle(ConfirmDonationReceivedCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var success = await _donationService.ConfirmDonationReceivedAsync(request.DonationId, request.CoordinatorId, cancellationToken);
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