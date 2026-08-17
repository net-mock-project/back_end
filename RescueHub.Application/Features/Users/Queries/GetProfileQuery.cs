using MediatR;
using Mapster;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Application.Contracts.Users;

namespace RescueHub.Application.Features.Users.Queries
{
    // Query lấy thông tin Profile của User hiện tại
    public record GetProfileQuery(
        Guid UserId
    ) : IRequest<UserProfileDto?>;

    public class GetProfileQueryHandler
        : IRequestHandler<GetProfileQuery, UserProfileDto?>
    {
        private readonly IUserService _userService;

        public GetProfileQueryHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserProfileDto?> Handle(
            GetProfileQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetProfileAsync(
                request.UserId,
                cancellationToken);

            return user?.Adapt<UserProfileDto>();
        }
    }
}