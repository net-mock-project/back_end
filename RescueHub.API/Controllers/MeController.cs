using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.Users;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Interfaces;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly IUserService _userService;

        // Inject UserService để xử lý nghiệp vụ User
        public MeController(IUserService userService)
        {
            _userService = userService;
        }

        // PATCH /api/me/profile
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Lấy UserId của người đang đăng nhập từ JWT
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. JWT không chứa UserId hợp lệ
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // 3. Gọi Service xử lý logic Update Profile
            var user = await _userService.UpdateProfileAsync(
                userId,
                request.FullName,
                request.Email,
                request.Phone,
                request.Province,
                cancellationToken);

            // 4. Map User Entity -> DTO trả về Frontend
            var result = new UserProfileDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Province = user.Province,
                UpdatedAt = user.UpdatedAt
            };

            // 5. Trả HTTP 200
            return Ok(result);
        }
    }
}