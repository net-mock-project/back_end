using MediatR;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models;
using RescueHub.API.Models.Authentication;
using RescueHub.Application.Features.Auth.Commands.Login;
using RescueHub.Application.Features.Auth.Commands.Register;
using RescueHub.Application.Features.Auth.Commands.ResendOtp;
using RescueHub.Application.Features.Auth.Commands.SendOtp;
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

    // 1. GỬI MÃ OTP VỀ EMAIL
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(
        [FromBody] SendOtpRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendOtpCommand(
            request.FullName,
            request.DateOfBirth,
            request.Email,
            request.Phone,
            request.Gender,
            request.Password,
            request.Address);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    // 2. GỬI LẠI MÃ OTP
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResendOtpCommand(
            request.Email);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    // 3. XÁC THỰC OTP VÀ ĐĂNG KÝ
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.OtpCode);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    // 4. ĐĂNG NHẬP
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

        // Trả thông báo về Client
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
}