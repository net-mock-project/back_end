using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Auth;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResultDto?>;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResultDto?>
{
    private readonly IAuthService _authService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IAuthService authService,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _authService = authService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResultDto?> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var (user, roleName) = await _authService.LoginAsync(
            request.Email,
            cancellationToken);

        if (user is null || roleName is null) return null;

        var isTruePassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isTruePassword)
        {
            return null;
        }

        var token = _jwtService.GenerateToken(
            user.Id,
            user.Email,
            user.RoleId,
            roleName);

        if (token is null) return null;

        return new LoginResultDto(token, user.Id, user.Email, user.RoleId, roleName);
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email format is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
