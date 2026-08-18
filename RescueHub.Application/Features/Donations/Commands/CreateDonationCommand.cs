using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Donation;
using RescueHub.Domain.Interfaces.Donations;

namespace RescueHub.Application.Features.Donations.Commands
{
    public record CreateDonationCommand(
        Guid DonatorId,
        string SupplyName,
        int Quantity,
        string Unit,
        DateTime DonationDate
    ) : IRequest<DonationDto?>;

    public class CreateDonationCommandHandler : IRequestHandler<CreateDonationCommand, DonationDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDonationService _donationService;

        public CreateDonationCommandHandler(IUnitOfWork unitOfWork, IDonationService donationService)
        {
            _unitOfWork = unitOfWork;
            _donationService = donationService;
        }

        public async Task<DonationDto?> Handle(CreateDonationCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await _donationService.CreateDonationAsync(
                    request.DonatorId,
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

    // Validator cho CreateDonationCommand
    public class CreateDonationCommandValidator : AbstractValidator<CreateDonationCommand>
    {
        public CreateDonationCommandValidator()
        {
            RuleFor(x => x.DonatorId)
                .NotEmpty()
                .WithMessage("Donator ID is required.");

            RuleFor(x => x.SupplyName)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("Supply name is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Unit)
                .NotEmpty()
                .WithMessage("Unit is required.");

            RuleFor(x => x.DonationDate)
                .NotEmpty()
                .WithMessage("Donation date is required.");
        }
    }
}