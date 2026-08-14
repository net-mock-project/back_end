using MediatR;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Auth.Commands.ResendOtp
{
    public record ResendOtpCommand(string Email) : IRequest<bool>;

    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, bool>
    {
        private readonly IAuthService _authService;

        public ResendOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ResendOtpAsync(request.Email);
        }
    }
}