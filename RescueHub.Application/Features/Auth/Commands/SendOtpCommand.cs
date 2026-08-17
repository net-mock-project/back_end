using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Auth;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICacheService _cacheService;

        private readonly IEmailService _emailService;

        public SendOtpCommandHandler(
            IAuthService authService,
            IPasswordHasher passwordHasher,
            ICacheService cacheService,
            IEmailService emailService)
        {
            _authService = authService;
            _passwordHasher = passwordHasher;
            _cacheService = cacheService;
            _emailService = emailService;
        }

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            await _authService.ValidateNewUserRegistrationAsync(request.Email, cancellationToken);

            var passwordHash = _passwordHasher.Hash(request.Password);
            var otpCode = Random.Shared.Next(100000, 999999).ToString();

            var pendingData = new PendingRegistrationDto(
                request.FullName,
                request.DateOfBirth,
                request.Email,
                request.PhoneNumber,
                request.Gender,
                passwordHash,
                request.Address,
                otpCode,
                DateTime.UtcNow.AddMinutes(5)
            );

            var cacheKey = GetCacheKey(request.Email);

            await _cacheService.SetAsync(cacheKey, pendingData, TimeSpan.FromMinutes(5));

            Console.WriteLine($"Mã OTP: {otpCode}");

            await _emailService.SendEmailAsync(
                request.Email,
                "Mã xác thực đăng ký RescueHub",
                $"Mã OTP của bạn là: {otpCode}. Có hiệu lực trong 5 phút."
            );

            return true;
        }

        private static string GetCacheKey(string email) => $"auth:pending-reg:{email.ToLower()}";
    }

    public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
    {
        public SendOtpCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .MaximumLength(100)
                .WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of birth is required.")
                .LessThan(DateTime.UtcNow)
                .WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email format is invalid.")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^(0|\+84)[0-9]{9,10}$")
                .WithMessage("Phone number is invalid.");

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Gender is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MaximumLength(255)
                .WithMessage("Address must not exceed 255 characters.");
        }
    }
}