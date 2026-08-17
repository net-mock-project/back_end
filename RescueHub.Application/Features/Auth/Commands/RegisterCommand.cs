using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Auth;
using RescueHub.Domain.Interfaces.Auth;

namespace RescueHub.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Email,
    string OtpCode
) : IRequest<bool>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly IAuthService _authService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IAuthService authService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            var pendingData = await _cacheService.GetAsync<PendingRegistrationDto>(GetCacheKey(request.Email));

            if (pendingData == null)
            {
                throw new InvalidOperationException(
                    "Mã OTP đã hết hạn hoặc không tồn tại.");
            }

            if (pendingData.OtpCode != request.OtpCode)
            {
                throw new ArgumentException(
                    "Mã OTP không chính xác.");
            }

            if (DateTime.UtcNow > pendingData.ExpiresAt)
            {
                throw new InvalidOperationException("Mã OTP đã hết hạn.");
            }

            await _authService.RegisterAsync(pendingData.Address, pendingData.FullName, request.Email, pendingData.PhoneNumber, pendingData.DateOfBirth, pendingData.Gender, pendingData.PasswordHash, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await _cacheService.RemoveAsync(request.Email);

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

    private static string GetCacheKey(string email) => $"auth:pending-reg:{email.ToLower()}";
}
