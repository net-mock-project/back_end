using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using System.Text.Json;

namespace RescueHub.Application.Features.Users.Commands;

// Command mở khóa tài khoản User
public record UnlockUserCommand(
    Guid UserId,
    Guid PerformedByUserId
) : IRequest<UserStatusDto?>;


// Handler xử lý mở khóa tài khoản
public class UnlockUserCommandHandler
    : IRequestHandler<UnlockUserCommand, UserStatusDto?>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    public UnlockUserCommandHandler(
        IUserService userService,
        IUnitOfWork unitOfWork,
        IAuditLogService auditLogService)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    public async Task<UserStatusDto?> Handle(
        UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            var oldUser = await _userService.GetUserDetailAsync(
    request.UserId,
    cancellationToken);

            if (oldUser == null)
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                return null;
            }

            var user = await _userService.UnlockUserAsync(
                request.UserId,
                cancellationToken);

            if (user == null)
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                return null;
            }

            if (oldUser.Status != user.Status)
            {
                var oldValue = JsonSerializer.Serialize(new
                {
                    status = oldUser.Status.ToString()
                });

                var newValue = JsonSerializer.Serialize(new
                {
                    status = user.Status.ToString()
                });

                var auditLog = new AuditLog(
                    request.PerformedByUserId,
                    "Unlock",
                    "User",
                    user.Id,
                    oldValue,
                    newValue);

                await _auditLogService.CreateAsync(
                    auditLog,
                    cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await _unitOfWork.CommitAsync(
                cancellationToken);

            return user.Adapt<UserStatusDto>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}


// Validate Command
public class UnlockUserCommandValidator
    : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}