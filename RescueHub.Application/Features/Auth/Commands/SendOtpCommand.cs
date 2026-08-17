using MediatR;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Application.Features.Auth.Commands
{
    public record SendOtpCommand(
        string FullName,
        DateTime DateOfBirth,
        string Email,
        string PhoneNumber,
        Gender Gender,
        string Password,
        string Address
    ) : IRequest<bool>;

    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly IAuthService _authService;

        public SendOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            return await _authService.SendOtpAsync(
                request.FullName,
                request.DateOfBirth,
                request.Email,
                request.PhoneNumber,
                request.Gender,
                request.Password,
                request.Address
            );
        }
    }
}