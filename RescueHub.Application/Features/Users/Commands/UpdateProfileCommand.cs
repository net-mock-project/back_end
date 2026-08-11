using Mapster;
using MediatR;
using RescueHub.Application.Contracts;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.Users.Commands
{
    // Command cập nhật thông tin Profile
    public record UpdateProfileCommand(
        Guid UserId,
        string? FullName,
        string? Phone,
        string? Province
    ) : IRequest<UserProfileDto?>;

    // Handler xử lý cập nhật Profile
    public class UpdateProfileCommandHandler
        : IRequestHandler<UpdateProfileCommand, UserProfileDto?>
    {
        private readonly IUserService _userService;

        public UpdateProfileCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserProfileDto?> Handle(
            UpdateProfileCommand request,
            CancellationToken cancellationToken)
        {
            // Cập nhật thông tin User
            var user = await _userService.UpdateProfileAsync(
                request.UserId,
                request.FullName,
                request.Phone,
                request.Province);

            return user?.Adapt<UserProfileDto>();
        }
    }
}