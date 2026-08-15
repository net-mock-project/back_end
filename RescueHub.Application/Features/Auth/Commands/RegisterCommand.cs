using MediatR;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Application.Features.Auth.Commands
{
    public record RegisterCommand(
        string Email,
        string OtpCode
    ) : IRequest<bool>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request.Email, request.OtpCode);
        }
    }
}