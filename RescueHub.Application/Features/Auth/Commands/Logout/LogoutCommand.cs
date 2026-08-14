using MediatR;

namespace RescueHub.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand : IRequest;

    public class LogoutCommandHandler
        : IRequestHandler<LogoutCommand>
    {
        public Task Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}