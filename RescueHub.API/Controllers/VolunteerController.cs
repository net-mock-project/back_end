using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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


        // 1. REQUESTER / VOLUNTEER PROFILE


        [HttpPost("api/volunteers/profile")]
        [Tags("Volunteer - Profile")]
        [Authorize(Roles = "Requester")]
        public async Task<IActionResult> SubmitProfile(
            [FromBody] SubmitVolunteerProfileRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = _mapper.Map<SubmitVolunteerProfileCommand>(request) with
            {
                UserId = userId.Value
            };

            var result = await _sender.Send(command, cancellationToken);
            if (result == null)
                throw new ConflictException("Volunteer profile already exists.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpGet("api/volunteers/profile")]
        [Tags("Volunteer - Profile")]
        public async Task<IActionResult> GetProfile(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var query = new GetVolunteerProfileQuery(userId.Value);
            var result = await _sender.Send(query, cancellationToken);

            if (result == null)
                throw new NotFoundException($"Volunteer profile for User '{userId}' not found.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpPut("api/volunteers/profile")]
        [Tags("Volunteer - Profile")]
        public async Task<IActionResult> UpdateVolunteerProfile(
            [FromBody] UpdateVolunteerProfileRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = _mapper.Map<UpdateVolunteerProfileCommand>(request) with
            {
                UserId = userId.Value
            };

            var result = await _sender.Send(command, cancellationToken);
            if (result == null)
                throw new NotFoundException("Volunteer profile was not found.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpDelete("api/volunteers/profile")]
        [Tags("Volunteer - Profile")]
        [Authorize(Roles = "Requester")]
        public async Task<IActionResult> CancelVolunteerProfile(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = new CancelVolunteerProfileCommand(userId.Value);
            var isSuccess = await _sender.Send(command, cancellationToken);

            if (!isSuccess)
                throw new NotFoundException("Volunteer application not found or cannot be cancelled.");

            return Ok(new { message = "Volunteer application cancelled successfully." });
        }

        // 2. COORDINATOR MANAGEMENT

        [HttpGet("api/coordinator/volunteers/pending")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetPendingVolunteerProfiles(
            [FromQuery] VolunteerQueryRequest request,
            CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var queryRequest = _mapper.Map<QueryRequest>(request);
            var criteria = _mapper.Map<QueryCriteria>(queryRequest);

            var result = await _sender.Send(
                new GetPendingVolunteerProfilesQuery(coordinatorId.Value, criteria),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("api/coordinator/volunteers")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetApprovedVolunteerProfiles(
            [FromQuery] VolunteerQueryRequest request,
            CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var queryRequest = _mapper.Map<QueryRequest>(request);
            var criteria = _mapper.Map<QueryCriteria>(queryRequest);

            var result = await _sender.Send(
                new GetApprovedVolunteerProfilesQuery(coordinatorId.Value, criteria),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("api/coordinator/volunteers/{id:guid}")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetVolunteerProfileById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var result = await _sender.Send(
                new GetVolunteerProfileByIdQuery(id, coordinatorId.Value),
                cancellationToken);

            if (result == null)
                throw new NotFoundException($"Volunteer profile with ID '{id}' was not found in your managed province.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpPost("api/coordinator/volunteers")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> CreateVolunteerByCoordinator(
            [FromBody] CoordinatorCreateVolunteerRequest request,
            CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = new CreateVolunteerByCoordinatorCommand(
                coordinatorId.Value,
                request.UserId,
                request.ExperienceYears,
                request.CVUrl,
                request.Skills.Select(s => new VolunteerSkillInput(s.SkillId, s.Level)).ToList());

            var result = await _sender.Send(command, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException("Cannot create volunteer. Ensure the user belongs to your managed province and has no active profile.");
            }

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpPut("api/coordinator/volunteers/{id:guid}")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> UpdateVolunteerByCoordinator(
            Guid id,
            [FromBody] UpdateVolunteerProfileRequest request,
            CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = new UpdateVolunteerByCoordinatorCommand(
                coordinatorId.Value,
                id,
                request.ExperienceYears,
                request.CVUrl,
                request.Skills.Select(s => new VolunteerSkillInput(s.SkillId, s.Level)).ToList());

            var result = await _sender.Send(command, cancellationToken);
            if (result == null)
                throw new NotFoundException($"Volunteer profile with ID '{id}' was not found in your managed province.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpDelete("api/coordinator/volunteers/{id:guid}")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> DeleteVolunteerByCoordinator(
            Guid id,
            [FromBody] RejectVolunteerProfileRequest? request,
            CancellationToken cancellationToken)
        {
            var coordinatorId = GetCurrentUserId();
            if (coordinatorId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = new DeleteVolunteerByCoordinatorCommand(
                coordinatorId.Value,
                id,
                request?.Reason);

            var isSuccess = await _sender.Send(command, cancellationToken);
            if (!isSuccess)
                throw new NotFoundException($"Volunteer profile with ID '{id}' was not found in your managed province.");

            return Ok(new { message = "Volunteer revoked successfully." });
        }

        [HttpPatch("api/coordinator/volunteers/{id:guid}/approve")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> ApproveProfile(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var approverId = GetCurrentUserId();
            if (approverId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = new ApproveVolunteerProfileCommand(id, approverId.Value);
            var result = await _sender.Send(command, cancellationToken);

            if (result == null)
                throw new NotFoundException($"Volunteer profile '{id}' not found or cannot be approved.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        [HttpPatch("api/coordinator/volunteers/{id:guid}/reject")]
        [Tags("Coordinator - Volunteers")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> RejectProfile(
            [FromRoute] Guid id,
            [FromBody] RejectVolunteerProfileRequest? request,
            CancellationToken cancellationToken)
        {
            var approverId = GetCurrentUserId();
            if (approverId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var command = new RejectVolunteerProfileCommand(
                id,
                approverId.Value,
                request?.Reason);

            var result = await _sender.Send(command, cancellationToken);

            if (result == null)
                throw new NotFoundException($"Volunteer profile '{id}' not found or cannot be rejected.");

            var response = _mapper.Map<VolunteerProfileResponse>(result);
            return Ok(response);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdValue, out var userId) ? userId : null;
        }
    }
}