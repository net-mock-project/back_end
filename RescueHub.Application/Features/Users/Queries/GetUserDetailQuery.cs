using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Interfaces.Users;

namespace RescueHub.Application.Features.Users.Queries
{
    public record GetUserDetailQuery(
        Guid UserId
    ) : IRequest<UserDetailDto?>;


    public class GetUserDetailQueryHandler
        : IRequestHandler<
            GetUserDetailQuery,
            UserDetailDto?>
    {
        private readonly IUserService _userService;

        public GetUserDetailQueryHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserDetailDto?> Handle(
            GetUserDetailQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserDetailAsync(
                request.UserId,
                cancellationToken);

            return user?.Adapt<UserDetailDto>();
        }
    }
}