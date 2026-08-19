using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.Users;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Features.Users.Commands;
using RescueHub.Application.Features.Users.Queries;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public AdminController(
            ISender sender,
            IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Lấy danh sách User có phân trang
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] GetUsersRequest request,
            CancellationToken cancellationToken)
        {
            var queryRequest =
                _mapper.Map<QueryRequest>(request);

            var query =
                new GetUsersQuery(queryRequest);

            var result = await _sender.Send(
                query,
                cancellationToken);

            var response =
                _mapper.Map<PaginationResponse<UserListResponse>>(result);

            return Ok(response);
        }

        // Lấy chi tiết User
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserDetail(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var query = new GetUserDetailQuery(userId);

            var result = await _sender.Send(
                query,
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"User '{userId}' not found.");
            }

            var response =
                _mapper.Map<UserDetailResponse>(result);

            return Ok(response);
        }

        // Admin tạo User mới
        [HttpPost]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            var adminUserId = GetCurrentUserId();

            if (adminUserId == null)
            {
                return Unauthorized();
            }

            var command =
                _mapper.Map<CreateUserCommand>(request)
                with
                {
                    PerformedByUserId = adminUserId.Value
                };

            var result = await _sender.Send(
                command,
                cancellationToken);

            var response =
                _mapper.Map<CreateUserResponse>(result);

            return Ok(response);
        }

        

        // Admin khóa tài khoản User
        [HttpPatch("{userId:guid}/lock")]
        public async Task<IActionResult> LockUser(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var adminUserId = GetCurrentUserId();

            if (adminUserId == null)
            {
                return Unauthorized();
            }

            var command =
                new LockUserCommand(
                    userId,
                    adminUserId.Value);

            var result = await _sender.Send(
                command,
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"User '{userId}' not found.");
            }

            var response =
                _mapper.Map<UserStatusResponse>(result);

            return Ok(response);
        }

        // Admin mở khóa tài khoản User
        [HttpPatch("{userId:guid}/unlock")]
        public async Task<IActionResult> UnlockUser(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var adminUserId = GetCurrentUserId();

            if (adminUserId == null)
            {
                return Unauthorized();
            }

            var command =
                new UnlockUserCommand(
                    userId,
                    adminUserId.Value);

            var result = await _sender.Send(
                command,
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"User '{userId}' not found.");
            }

            var response =
                _mapper.Map<UserStatusResponse>(result);

            return Ok(response);
        }

        // Lấy UserId của Admin từ token
        private Guid? GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}