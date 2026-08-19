using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.Volunteers;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Features.Volunteers.Commands;
using RescueHub.Application.Features.Volunteers.Queries;
using RescueHub.Domain.Common.Querying;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/volunteers")]
    [Authorize]
    public class VolunteersController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public VolunteersController(
            ISender sender,
            IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Đăng ký hồ sơ Volunteer
        [HttpPost("profile")]
        [Authorize(Roles = "Requester")]
        public async Task<IActionResult> SubmitProfile(
            [FromBody] SubmitVolunteerProfileRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var command = _mapper
                .Map<SubmitVolunteerProfileCommand>(request)
                with
            {
                UserId = userId.Value
            };

            var result = await _sender.Send(
                command,
                cancellationToken);

            if (result == null)
            {
                throw new ConflictException(
                    "Volunteer profile already exists.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Cập nhật lại hồ sơ Volunteer
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateVolunteerProfile(
            [FromBody] UpdateVolunteerProfileRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var command = _mapper.Map<UpdateVolunteerProfileCommand>(request) with
            {
                UserId = userId.Value
            };

            var result = await _sender.Send(command, cancellationToken);
            if (result == null)
            {
                throw new NotFoundException("Volunteer profile was not found.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Hủy đơn đăng ký hồ sơ Volunteer đang chờ duyệt
        [HttpDelete("profile")]
        [Authorize(Roles = "Requester")]
        public async Task<IActionResult> CancelVolunteerProfile(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var command = new CancelVolunteerProfileCommand(userId.Value);
            var isSuccess = await _sender.Send(command, cancellationToken);

            if (!isSuccess)
            {
                throw new NotFoundException(
                    "Volunteer application not found or cannot be cancelled.");
            }

            return Ok(new { message = "Volunteer application cancelled successfully." });
        }

        // Lấy hồ sơ Volunteer của User hiện tại
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var query = new GetVolunteerProfileQuery(userId.Value);
            var result = await _sender.Send(query, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Volunteer profile for User '{userId}' not found.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Coordinator lấy danh sách hồ sơ chờ duyệt (phân trang, lọc, tìm kiếm)
        [HttpGet("pending")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetPendingVolunteerProfiles(
            [FromQuery] VolunteerQueryRequest request,
            CancellationToken cancellationToken)
        {
            var queryRequest = _mapper.Map<QueryRequest>(request);
            var criteria = _mapper.Map<QueryCriteria>(queryRequest);

            var result = await _sender.Send(
                new GetPendingVolunteerProfilesQuery(criteria),
                cancellationToken);

            return Ok(result);
        }

        // Coordinator lấy danh sách Volunteer chính thức đã được duyệt
        [HttpGet]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetApprovedVolunteerProfiles(
            [FromQuery] VolunteerQueryRequest request,
            CancellationToken cancellationToken)
        {
            var queryRequest = _mapper.Map<QueryRequest>(request);
            var criteria = _mapper.Map<QueryCriteria>(queryRequest);

            var result = await _sender.Send(
                new GetApprovedVolunteerProfilesQuery(criteria),
                cancellationToken);

            return Ok(result);
        }

        // Coordinator xem chi tiết một hồ sơ Volunteer bất kỳ theo Id
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetVolunteerProfileById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetVolunteerProfileByIdQuery(id),
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Volunteer profile with ID '{id}' was not found.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Coordinator duyệt hồ sơ Volunteer
        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> ApproveProfile(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var approverId = GetCurrentUserId();
            if (approverId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var command = new ApproveVolunteerProfileCommand(id, approverId.Value);
            var result = await _sender.Send(command, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Volunteer profile '{id}' not found or cannot be approved.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Coordinator từ chối hồ sơ Volunteer kèm lý do tùy chọn
        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> RejectProfile(
            [FromRoute] Guid id,
            [FromBody] RejectVolunteerProfileRequest? request,
            CancellationToken cancellationToken)
        {
            var approverId = GetCurrentUserId();
            if (approverId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var command = new RejectVolunteerProfileCommand(
                id,
                approverId.Value,
                request?.Reason);

            var result = await _sender.Send(command, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Volunteer profile '{id}' not found or cannot be rejected.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Lấy UserId từ Claims của JWT token
        private Guid? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}