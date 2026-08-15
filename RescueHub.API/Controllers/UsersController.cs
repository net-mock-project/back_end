using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.Users;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Features.Users.Commands;
using RescueHub.Application.Features.Users.Queries;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public UsersController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Lấy thông tin Profile của User hiện tại
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetProfileQuery(
                userId.Value);

            var result = await _sender.Send(
                query,
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"User '{userId}' not found.");
            }

            var response = _mapper.Map<GetProfileResponse>(result);

            return Ok(response);
        }

        // Cập nhật thông tin Profile của User hiện tại
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }


            // Map Request sang Command và gắn UserId từ token
            var command = _mapper.Map<UpdateProfileCommand>(request)
                with
            {
                UserId = userId.Value
            };

            var result = await _sender.Send(command);

            if (result == null)
            {
                throw new NotFoundException(
                    $"User '{userId}' not found.");
            }

            // Map DTO sang Response
            var response = _mapper.Map<UserProfileResponse>(result);

            return Ok(response);
        }


        // Cập nhật ảnh đại diện (avatar) của User hiện tại
        [HttpPatch("profile/avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAvatar(
            [FromForm] UpdateAvatarRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }


            // Mở Stream từ file ảnh để truyền xuống Application
            using var fileStream = request.Avatar.OpenReadStream();

            var command = new UpdateAvatarCommand(
                userId.Value,
                fileStream,
                request.Avatar.FileName);

            var profileUrl = await _sender.Send(
                command,
                cancellationToken);

            if (profileUrl == null)
            {
                throw new NotFoundException("User not found.");
            }

            var response = new UpdateAvatarResponse
            {
                ProfileUrl = profileUrl
            };

            return Ok(response);
        }

        // Lấy UserId từ token đăng nhập
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