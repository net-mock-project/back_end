using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using System.Text.Json;

namespace RescueHub.Application.Features.Users.Commands;

// Command Admin cập nhật thông tin User
public record UpdateUserCommand(
    Guid UserId,
    string? FullName,
    string? Phone,
    DateOnly? DateOfBirth,
    Gender? Gender,
    Guid PerformedByUserId
) : IRequest<UpdateUserDto?>;


// Handler xử lý cập nhật User
public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, UpdateUserDto?>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    public UpdateUserCommandHandler(
        IUserService userService,
        IUnitOfWork unitOfWork,
        IAuditLogService auditLogService)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    public async Task<UpdateUserDto?> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            var oldUser = await _userService.GetProfileAsync(
            request.UserId,
            cancellationToken);

            if (oldUser == null)
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                return null;
            }

            var user =
                await _userService.UpdateUserAsync(
                    request.UserId,
                    request.FullName,
                    request.Phone,
                    request.DateOfBirth,
                    request.Gender,
                    cancellationToken);

            if (user == null)
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                return null;
            }

            var oldValue = JsonSerializer.Serialize(new
            {
                fullName = oldUser.FullName,
                phone = oldUser.Phone,
                dateOfBirth = oldUser.DateOfBirth,
                gender = oldUser.Gender?.ToString()
            });

            var newValue = JsonSerializer.Serialize(new
            {
                fullName = user.FullName,
                phone = user.Phone,
                dateOfBirth = user.DateOfBirth,
                gender = user.Gender?.ToString()
            });

            if (oldValue != newValue)
            {
                var auditLog = new AuditLog(
                    request.PerformedByUserId,
                    "Update",
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

            return user.Adapt<UpdateUserDto>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}


// Validate dữ liệu Admin cập nhật User
public class UpdateUserCommandValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.PerformedByUserId)
            .NotEmpty()
            .WithMessage("PerformedByUserId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.FullName)
            .MaximumLength(100)
            .WithMessage(
                "Full name must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage(
                "Phone number is invalid.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(
                DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage(
                "Date of birth must be in the past.");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => x.Gender.HasValue)
            .WithMessage(
                "Gender is invalid.");
    }
}