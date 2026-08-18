using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.Donation;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Features.Donations.Commands;
using RescueHub.Application.Features.Donations.Queries;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/donation")]
    [Authorize]
    public class DonationController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DonationController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Lấy danh sách Donation của User hiện tại
        [HttpGet("me")]
        public async Task<IActionResult> GetMyDonation(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetMyDonationQuery(userId.Value);

            var result = await _sender.Send(query, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException($"Donations for user '{userId}' not found.");
            }

            var response = _mapper.Map<List<GetMyDonationResponse>>(result);

            return Ok(response);
        }

        // Tạo donation mới
        [HttpPost("create")]
        public async Task<IActionResult> CreateDonation(
            [FromBody] CreateDonationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = _mapper.Map<CreateDonationCommand>(request)
                with
            {
                DonatorId = userId.Value
            };

            var result = await _sender.Send(command, cancellationToken);
            if (result == null)
            {
                throw new NotFoundException($"Could not create donation for user '{userId}'.");
            }

            var response = _mapper.Map<GetMyDonationResponse>(result);
            return Ok(response);
        }

        // Cập nhật thông tin của đơn donation
        [HttpPut("{donationId}")]
        public async Task<IActionResult> UpdateDonation(
            [FromBody] UpdateDonationRequest request,
            Guid donationId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            // Map Request sang Command và gắn UserId, DonationId từ route/token
            var command = _mapper.Map<UpdateDonationCommand>(request)
                with
            {
                UserId = userId.Value,
                DonationId = donationId
            };

            var result = await _sender.Send(command, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException($"Donation '{donationId}' not found or cannot be updated.");
            }

            var response = _mapper.Map<GetMyDonationResponse>(result);

            return Ok(response);
        }

        // --- BỔ SUNG: Hủy đơn donation ---
        [HttpPatch("{donationId}/cancel")]
        public async Task<IActionResult> CancelDonation(
            Guid donationId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new CancelDonationCommand(userId.Value, donationId);
            var success = await _sender.Send(command, cancellationToken);

            if (!success)
            {
                throw new NotFoundException($"Donation '{donationId}' not found or cannot be cancelled.");
            }

            return Ok(new { message = "Đã hủy đơn quyên góp thành công." });
        }

        // Lấy UserId từ token đăng nhập
        private Guid? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}