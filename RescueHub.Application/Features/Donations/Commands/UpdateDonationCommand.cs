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
        string? SupplyName,
        int? Quantity,
        string? Unit,
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
                var result = await _donationService.UpdateDonationAsync(
                    request.UserId,
                    request.DonationId,
                    request.SupplyName,
                    request.Quantity,
                    request.Unit,
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

    // Validator cho UpdateDonationCommand (chỉ validate các trường khi chúng được truyền lên)
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

            RuleFor(x => x.SupplyName)
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.SupplyName))
                .WithMessage("Supply name must not exceed 150 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .When(x => x.Quantity.HasValue)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Unit)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Unit))
                .WithMessage("Unit must not exceed 50 characters.");
        }
    }
}