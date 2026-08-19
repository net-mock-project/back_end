using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Features.Donations.Commands;
using RescueHub.Application.Features.Donations.Queries;
using System.Security.Claims;
using RescueHub.API.Models.Donation;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/coordinator")]
    [Authorize(Roles = "Coordinator")] // Chỉ tài khoản có quyền Coordinator mới gọi được
    public class CoordinatorDonationController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CoordinatorDonationController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        
        [HttpGet("donations")]
        public async Task<IActionResult> GetAllDonations(CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null) return Unauthorized();

            var query = new GetAllDonationsQuery(coordinatorId.Value);
            var result = await _sender.Send(query, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException($"Donations for coordinatorId '{coordinatorId}' not found.");
            }

            var response = _mapper.Map<List<GetMyDonationResponse>>(result);

            return Ok(response);
        }

        // 2. Xác nhận đồ đã đến kho (Chuyển Pending -> completed)
        [HttpPatch("donations/{donationId:guid}/accept")]
        public async Task<IActionResult> ConfirmCompleted(Guid donationId, CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null) return Unauthorized();

            var command = new ConfirmDonationReceivedCommand(donationId, coordinatorId.Value);
            var success = await _sender.Send(command, cancellationToken);

            if (!success)
            {
                throw new NotFoundException($"Donation '{donationId}' not found or cannot be updated.");
            }

            return Ok(new { message = "Đã xác nhận nhận đồ thành công, trạng thái chuyển sang Received." });
        }

        [HttpPatch("donations/{donationId:guid}/reject")]
        public async Task<IActionResult> ConfirmRejected(Guid donationId, CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null) return Unauthorized();

            var command = new ConfirmDonationRejectedCommand(donationId, coordinatorId.Value);
            var success = await _sender.Send(command, cancellationToken);

            if (!success)
            {
                throw new NotFoundException($"Donation '{donationId}' not found or cannot be updated.");
            }

            return Ok(new { message = "Đã xác nhận từ chối quyên góp thành công." });
        }

        private Guid? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdValue, out var userId) ? userId : null;
        }
    }
}