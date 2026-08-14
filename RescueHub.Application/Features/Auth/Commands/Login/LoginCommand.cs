using MediatR;
using RescueHub.Application.Contracts;
using RescueHub.Application.Contracts.Authentication;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Services;

namespace RescueHub.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResultDto?>;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResultDto?>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IAuthService authService,
        IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;

    }

    public async Task<LoginResultDto?> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Cập nhật thông tin User
            var (token, user) = await _authService.LoginAsync(
                request.Email,
                request.Password,
                cancellationToken);

            if (user is null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return null;
            }


            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.SaveChangesAsync(
            cancellationToken);

            // Xác nhận transaction
            await _unitOfWork.CommitAsync(cancellationToken);


            return new LoginResultDto(token, user.Id, user.Email, user.RoleId);

        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

    }
}