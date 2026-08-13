using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models;
using RescueHub.API.Models.Authentication;
using RescueHub.Application.Features.Auth.Commands.Login;
namespace RescueHub.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. ĐĂNG NHẬP
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Email hoặc mật khẩu không đúng."
            });
        }

        // Lưu JWT vào HttpOnly Cookie
        Response.Cookies.Append(
            "rescuehub_token",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                IsEssential = true
            });

        // Không trả JWT về Client
        return Ok(new
        {
            message = "Login successful."
        });
    }

    // 5. ĐĂNG XUẤT
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(
            "rescuehub_token",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        return Ok(new
        {
            message = "Logout successful."
        });
    }

    // 5. KIỂM TRA TOKEN TRONG COOKIE
    [Authorize]
    [HttpGet("check-token")]
    public IActionResult CheckToken()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;

        return Ok(new
        {
            message = "Token hợp lệ.",
            userId,
            email,
            role
        });
    }
}