using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Donation;
using RescueHub.Domain.Interfaces.Donations;

namespace RescueHub.Application.Features.Donations.Commands
{
    public record UpdateDonationCommand(
        Guid UserId,
        Guid DonationId,
        List<DonationItemRequest?> Items,
        DateTime? DonationDate
        ) : IRequest<DonationDto?>;

    public class UpdateDonationCommandHandler : IRequestHandler<UpdateDonationCommand, DonationDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDonationService _donationService;

        public UpdateDonationCommandHandler(IUnitOfWork unitOfWork, IDonationService donationService)
        {
            _unitOfWork = unitOfWork;
            _donationService = donationService;
        }

        public async Task<DonationDto?> Handle(UpdateDonationCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Chuyển đổi từ List<DonationItemRequest?> sang List<(string SupplyName, int Quantity, string Unit)> (xử lý lọc null)
                var itemsTuple = request.Items?
                    .Where(i => i != null)
                    .Select(i => (i!.SupplyName, i.Quantity, i.Unit))
                    .ToList();

                var result = await _donationService.UpdateDonationAsync(
                    request.UserId,
                    request.DonationId,
                    itemsTuple,
                    request.DonationDate,
                    cancellationToken
                );

                if (result == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return null;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return result.Adapt<DonationDto>();
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    // Validator cho UpdateDonationCommand
    public class UpdateDonationCommandValidator : AbstractValidator<UpdateDonationCommand>
    {
        public UpdateDonationCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.DonationId)
                .NotEmpty()
                .WithMessage("Donation ID is required.");
        }
    }
}