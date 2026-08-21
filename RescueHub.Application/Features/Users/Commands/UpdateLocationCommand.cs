using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Features.Donations.Commands;
using RescueHub.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueHub.Application.Features.Users.Commands
{
    public record UpdateLocationCommand(
    Guid UserId,
    double Latitude,
    double Longitude
) : IRequest<bool>;

    public class UpdateLocationCommandHandler
    : IRequestHandler<UpdateLocationCommand, bool>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLocationCommandHandler(
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> Handle(
            UpdateLocationCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var user = await _userService.UpdateLocationAsync(
                    request.UserId,
                    request.Latitude,
                    request.Longitude,
                    cancellationToken);

                if (user == null)
                {
                    await _unitOfWork.RollbackAsync(
                        cancellationToken);

                    return false;
                }

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                await _unitOfWork.CommitAsync(
                    cancellationToken);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }



    public class UpdateLocationCommandValidator
        : AbstractValidator<UpdateLocationCommand>
    {
        public UpdateLocationCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage(
                    "Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage(
                    "Longitude must be between -180 and 180.");
        }
    }
}
