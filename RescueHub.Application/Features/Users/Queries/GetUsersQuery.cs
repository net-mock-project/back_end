using FluentValidation;
using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Interfaces.Users;

namespace RescueHub.Application.Features.Users.Queries
{
    public record GetUsersQuery(
        QueryRequest Request
    ) : IRequest<PaginationResponse<UserListDto>>;


    public class GetUsersQueryHandler
        : IRequestHandler<
            GetUsersQuery,
            PaginationResponse<UserListDto>>
    {
        private readonly IUserService _userService;

        public GetUsersQueryHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<PaginationResponse<UserListDto>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            // Chuyển request phân trang sang Domain criteria
            var criteria =
                request.Request.Adapt<QueryCriteria>();

            var result = await _userService.GetUsersAsync(
                criteria,
                cancellationToken);

            // Chuyển read model sang DTO
            var items =
                result.Items.Adapt<List<UserListDto>>();

            return new PaginationResponse<UserListDto>(
                items,
                result.TotalCount,
                criteria.PageNumber,
                criteria.PageSize);
        }
    }


    public class GetUsersQueryValidator
        : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
            RuleFor(x => x.Request.PageNumber)
                .GreaterThan(0)
                .WithMessage(
                    "PageNumber must be greater than 0.");

            RuleFor(x => x.Request.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage(
                    "PageSize must be between 1 and 100.");
        }
    }
}