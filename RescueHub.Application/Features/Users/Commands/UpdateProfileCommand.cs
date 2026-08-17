using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.Users;

namespace RescueHub.Application.Features.Users.Commands;

// Command cập nhật thông tin Profile
public record UpdateProfileCommand(
    Guid UserId,
    string? FullName,
    string? Phone,
    DateOnly? DateOfBirth,
    Gender? Gender
) : IRequest<UserProfileDto?>;

// Handler xử lý cập nhật Profile
public class UpdateProfileCommandHandler
    : IRequestHandler<UpdateProfileCommand, UserProfileDto?>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUserService userService, IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto?> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Cập nhật thông tin User
            var user = await _userService.UpdateProfileAsync(
                request.UserId,
                request.FullName,
                request.Phone,
                request.DateOfBirth,
                request.Gender,
                cancellationToken);

            if (user == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return null;
            }


            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.SaveChangesAsync(
            cancellationToken);

            // Xác nhận transaction
            await _unitOfWork.CommitAsync(cancellationToken);


            return user?.Adapt<UserProfileDto>();

        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

public class UpdateProfileCommandValidator
    : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number is invalid.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => x.Gender.HasValue)
            .WithMessage("Gender is invalid.");
    }
}
