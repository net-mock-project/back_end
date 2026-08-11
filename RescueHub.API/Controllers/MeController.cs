using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Features.Users.Commands;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly ISender _sender;

        public MeController(ISender sender)
        {
            _sender = sender;
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

            var command = new UpdateProfileCommand(
                userId.Value,
                request.FullName,
                request.Phone,
                request.Province);

            var result = await _sender.Send(command);

            if (result == null)
            {
                throw new NotFoundException(
                    $"User '{userId}' not found.");
            }

            return Ok(result);
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