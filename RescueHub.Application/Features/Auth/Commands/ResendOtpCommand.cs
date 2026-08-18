using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Auth;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Application.Features.Auth.Commands;

public record ResendOtpCommand(string Email) : IRequest<bool>;

public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, bool>
{
    private readonly ICacheService _cacheService;
    private readonly IEmailService _emailService;

    public ResendOtpCommandHandler(
        ICacheService cacheService,
        IEmailService emailService)
    {
        _cacheService = cacheService;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var pendingData = await _cacheService.GetAsync<PendingRegistrationDto>(GetCacheKey(request.Email));

        if (pendingData == null)
        {
            throw new InvalidOperationException(
                "Yêu cầu đăng ký không tồn tại hoặc đã hết hạn. Vui lòng đăng ký lại.");
        }
        if (pendingData == null) return false;

        var newOtpCode = new Random().Next(100000, 999999).ToString();
        var updatedData = pendingData with
        {
            OtpCode = newOtpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        await _cacheService.SetAsync(
            GetCacheKey(request.Email),
            updatedData,
            TimeSpan.FromMinutes(5));

        await _emailService.SendEmailAsync(request.Email, "Mã xác thực OTP mới", $"Mã OTP mới của bạn là: {newOtpCode}.");

        return true;
    }

    private static string GetCacheKey(string email) => $"auth:pending-reg:{email.ToLower()}";

    public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
    {
        public ResendOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email format is invalid.")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters.");
        }
    }
}
