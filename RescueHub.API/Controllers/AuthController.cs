using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models;
using System.Threading.Tasks;
using RescueHub.Application.Features.Auth.Commands.SendOtp;
using RescueHub.Application.Features.Auth.Commands.ResendOtp;
using RescueHub.Application.Features.Auth.Commands.Register;

namespace RescueHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public AuthController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // --- API 1: GỬI MÃ OTP VỀ Email ---
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            var command = _mapper.Map<SendOtpCommand>(request);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        // --- API 2: GỬI LẠI MÃ OTP ---
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var command = _mapper.Map<ResendOtpCommand>(request);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        // --- API 3: XÁC THỰC OTP VÀ ĐĂNG KÝ CHÍNH THỨC ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = _mapper.Map<RegisterCommand>(request);
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}