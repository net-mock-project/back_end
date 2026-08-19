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
                return Unauthorized();
            }

            // Map Request sang Command và gắn UserId từ token
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

            // Map DTO sang Response
            var response =
                _mapper.Map<VolunteerProfileResponse>(result);

            return Ok(response);
        }

        // Lấy hồ sơ Volunteer của User hiện tại
        [HttpGet("profile")]
        [Authorize(Roles = "Requester")]
        public async Task<IActionResult> GetProfile(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetVolunteerProfileQuery(
                userId.Value);

            var result = await _sender.Send(
                query,
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Volunteer profile for User '{userId}' not found.");
            }

            var response =
                _mapper.Map<VolunteerProfileResponse>(result);

            return Ok(response);
        }

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
                return Unauthorized();
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

        // Coordinator từ chối hồ sơ Volunteer
        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> RejectProfile(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var approverId = GetCurrentUserId();
            if (approverId == null)
            {
                return Unauthorized();
            }

            var command = new RejectVolunteerProfileCommand(id, approverId.Value);
            var result = await _sender.Send(command, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Volunteer profile '{id}' not found or cannot be rejected.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        // Lấy UserId từ token đăng nhập
        private Guid? GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(
                userIdValue,
                out var userId)
                ? userId
                : null;
        }
    }
}