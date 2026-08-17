using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Interfaces.Users;

namespace RescueHub.Application.Features.Users.Commands
{
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
}