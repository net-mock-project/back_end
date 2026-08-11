using Microsoft.AspNetCore.Mvc;
using RescueHub.Domain.Entities.RegisterDTOs;
using RescueHub.Domain.Interfaces;
using System.Threading.Tasks;

namespace RescueHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // --- API 1: GỬI MÃ OTP VỀ SỐ ĐIỆN THOẠI ---
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.SendOtpAsync(dto.Email);

            if (!success)
            {
                return BadRequest(new { message = "Email này đã được đăng ký hoặc không thể gửi mã lúc này." });
            }

            return Ok(new { message = "Mã OTP đã được gửi thành công đến email của bạn!" });
        }


        // --- API 2: GỬI LẠI MÃ OTP ---
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] SendOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.ResendOtpAsync(dto.Email);

            if (!success)
            {
                return BadRequest(new { message = "Không thể gửi mã OTP mới." });
            }

            return Ok(new { message = "Mã OTP mới đã được gửi thành công đến email của bạn!" });
        }

        // --- API 3: XÁC THỰC OTP VÀ ĐĂNG KÝ CHÍNH THỨC ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Kiểm tra Validation từ DTO (bao gồm cả trường OtpCode 6 số)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi AuthService (bên trong sẽ tự kiểm tra OTP và lưu User)
            var result = await _authService.VerifyAndRegisterAsync(dto);

            if (result == "InvalidOtp")
            {
                return BadRequest(new { message = "Mã OTP không chính xác hoặc đã hết hạn!" });
            }

            if (result == "UserExists")
            {
                return BadRequest(new { message = "Email hoặc số điện thoại này đã được sử dụng!" });
            }

            if (result == null) 
            {
                return BadRequest(new { message = "Đăng ký thất bại, vui lòng thử lại sau." });
            }

            return Ok(new { message = "Đăng ký tài khoản thành công!" });
        }
    }
}