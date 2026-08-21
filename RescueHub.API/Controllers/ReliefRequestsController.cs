using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models;
using RescueHub.API.Models.ReliefRequests;
using RescueHub.Application.Features.ReliefRequests.Commands;
using RescueHub.Application.Features.ReliefRequests.Queries;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/relief-requests")]
    [Authorize]
    public class ReliefRequestsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ReliefRequestsController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        private Guid? GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }

        private bool IsCoordinator() =>
            User.IsInRole("Coordinator") || User.IsInRole("Admin");

        [HttpGet]
        public async Task<IActionResult> GetRequests(
            [FromQuery] bool mine = false,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            if (mine)
            {
                var myList = await _sender.Send(new GetMyReliefRequestsQuery(userId.Value), cancellationToken);
                var myResponse = _mapper.Map<List<ReliefRequestResponse>>(myList);
                return Ok(ApiResponse.Success(myResponse));
            }

            var allList = await _sender.Send(new GetAllReliefRequestsQuery(), cancellationToken);
            var allResponse = _mapper.Map<List<ReliefRequestResponse>>(allList);
            return Ok(ApiResponse.Success(allResponse));
        }

        [HttpGet("{requestId:guid}")]
        public async Task<IActionResult> GetRequest(
            Guid requestId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var isCoordinator = IsCoordinator();

            var dto = await _sender.Send(
                new GetReliefRequestByIdQuery(requestId, userId.Value, isCoordinator),
                cancellationToken);

            var response = _mapper.Map<ReliefRequestResponse>(dto);
            return Ok(ApiResponse.Success(response));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(
            [FromBody] CreateReliefRequestApiRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var command = _mapper.Map<CreateReliefRequestCommand>(request) with
            {
                RequesterId = userId.Value
            };

            var dto = await _sender.Send(command, cancellationToken);
            var response = _mapper.Map<ReliefRequestResponse>(dto);
            return Ok(ApiResponse.Success(response));
        }

        [HttpPatch("{requestId:guid}")]
        public async Task<IActionResult> UpdateRequest(
            Guid requestId,
            [FromBody] UpdateReliefRequestApiRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var isCoordinator = IsCoordinator();

            var command = _mapper.Map<UpdateReliefRequestCommand>(request) with
            {
                RequestId = requestId,
                UserId = userId.Value,
                IsCoordinator = isCoordinator
            };

            var dto = await _sender.Send(command, cancellationToken);
            var response = _mapper.Map<ReliefRequestResponse>(dto);
            return Ok(ApiResponse.Success(response));
        }

        [HttpDelete("{requestId:guid}")]
        public async Task<IActionResult> CancelRequest(
            Guid requestId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _sender.Send(new CancelReliefRequestCommand(requestId, userId.Value), cancellationToken);
            return Ok(ApiResponse.Success(new { message = "Đã hủy yêu cầu cứu trợ thành công." }));
        }

        [HttpPut("{requestId:guid}/approve")]
        [Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> ApproveRequest(Guid requestId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _sender.Send(new CoordinatorActionReliefRequestCommand(requestId, userId.Value, "approve"), cancellationToken);
            return Ok(ApiResponse.Success(new { message = "Đã duyệt yêu cầu cứu trợ thành công." }));
        }

        [HttpPut("{requestId:guid}/reject")]
        [Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> RejectRequest(Guid requestId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _sender.Send(new CoordinatorActionReliefRequestCommand(requestId, userId.Value, "reject"), cancellationToken);
            return Ok(ApiResponse.Success(new { message = "Đã từ chối yêu cầu cứu trợ thành công." }));
        }

        [HttpPut("{requestId:guid}/complete")]
        [Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> CompleteRequest(Guid requestId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _sender.Send(new CoordinatorActionReliefRequestCommand(requestId, userId.Value, "complete"), cancellationToken);
            return Ok(ApiResponse.Success(new { message = "Đã hoàn thành yêu cầu cứu trợ thành công." }));
        }

        [HttpPut("{requestId:guid}/report")]
        [Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> ReportRequest(Guid requestId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _sender.Send(new CoordinatorActionReliefRequestCommand(requestId, userId.Value, "report"), cancellationToken);
            return Ok(ApiResponse.Success(new { message = "Đã báo cáo yêu cầu cứu trợ thành công." }));
        }

        [HttpPut("{requestId:guid}/export")]
        [Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> ExportRequest(Guid requestId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _sender.Send(new CoordinatorActionReliefRequestCommand(requestId, userId.Value, "export"), cancellationToken);
            return Ok(ApiResponse.Success(new { message = "Đã xuất yêu cầu cứu trợ thành công." }));
        }
        [HttpPost("{id}/availability")]
        [Authorize]
        public async Task<IActionResult> RegisterAvailability(Guid id, CancellationToken cancellationToken)
        {
            var volunteerIdStr = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(volunteerIdStr) || !Guid.TryParse(volunteerIdStr, out var volunteerId))
                return Unauthorized();

            var command = new RescueHub.Application.Features.VolunteerEngagements.Commands.RegisterAvailabilityCommand(volunteerId, id);
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id}/availability")]
        [Authorize]
        public async Task<IActionResult> CancelAvailability(Guid id, CancellationToken cancellationToken)
        {
            var volunteerIdStr = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(volunteerIdStr) || !Guid.TryParse(volunteerIdStr, out var volunteerId))
                return Unauthorized();

            var command = new RescueHub.Application.Features.VolunteerEngagements.Commands.CancelAvailabilityCommand(volunteerId, id);
            var result = await _sender.Send(command, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
