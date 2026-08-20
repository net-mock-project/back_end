using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Application.Features.ReliefTasks.Commands;
using RescueHub.Application.Features.ReliefTasks.Queries;
using RescueHub.Application.Features.TaskAssignments.Commands;
using RescueHub.API.Models;
using RescueHub.API.Models.ReliefTasks;
using System.Security.Claims;

namespace RescueHub.API.Controllers;

[ApiController]
public class ReliefTasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ReliefTasksController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    // ==========================================
    // 1. RELIEF TASK CRUD (Coordinator)
    // ==========================================
    [HttpPost("api/relief-requests/{requestId}/tasks")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> CreateTask(Guid requestId, [FromBody] CreateReliefTaskApiRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateReliefTaskCommand>(request) with { RequestId = requestId };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse.Success(_mapper.Map<ReliefTaskResponse>(result)));
    }

    [HttpGet("api/relief-requests/{requestId}/tasks")]
    [Authorize]
    public async Task<IActionResult> GetTasks(Guid requestId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReliefTasksByRequestQuery(requestId), cancellationToken);
        return Ok(ApiResponse.Success(_mapper.Map<IEnumerable<ReliefTaskResponse>>(result)));
    }

    [HttpGet("api/relief-requests/{requestId}/tasks/{taskId}")]
    [Authorize]
    public async Task<IActionResult> GetTask(Guid requestId, Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReliefTaskByIdQuery(requestId, taskId), cancellationToken);
        if (result == null) return NotFound(ApiResponse.Fail(System.Net.HttpStatusCode.NotFound, "Task not found"));
        return Ok(ApiResponse.Success(_mapper.Map<ReliefTaskResponse>(result)));
    }

    [HttpPatch("api/relief-requests/{requestId}/tasks/{taskId}")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> UpdateTask(Guid requestId, Guid taskId, [FromBody] UpdateReliefTaskApiRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateReliefTaskCommand>(request) with { Id = taskId };
        var result = await _mediator.Send(command, cancellationToken);
        if (result == null) return NotFound(ApiResponse.Fail(System.Net.HttpStatusCode.NotFound, "Task not found"));
        return Ok(ApiResponse.Success(_mapper.Map<ReliefTaskResponse>(result)));
    }

    [HttpDelete("api/relief-requests/{requestId}/tasks/{taskId}")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> DeleteTask(Guid requestId, Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteReliefTaskCommand(taskId), cancellationToken);
        if (!result) return NotFound(ApiResponse.Fail(System.Net.HttpStatusCode.NotFound, "Task not found"));
        return Ok(ApiResponse.Success(new { message = "Deleted successfully" }));
    }

    [HttpPut("api/relief-requests/{requestId}/tasks/{taskId}/complete")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> CompleteTask(Guid requestId, Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteReliefTaskCommand(taskId), cancellationToken);
        if (!result) return NotFound(ApiResponse.Fail(System.Net.HttpStatusCode.NotFound, "Task not found"));
        return Ok(ApiResponse.Success(new { message = "Task completed" }));
    }

    // ==========================================
    // 2. TASK ASSIGNMENTS (Coordinator)
    // ==========================================
    [HttpGet("api/relief-requests/{requestId}/tasks/{taskId}/suitable-volunteers")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> GetSuitableVolunteers(Guid requestId, Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSuitableVolunteersQuery(requestId, taskId), cancellationToken);
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost("api/relief-requests/{requestId}/tasks/{taskId}/assignments/invite")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> InviteVolunteer(Guid requestId, Guid taskId, [FromBody] AssignTaskApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new AssignTaskCommand(taskId, request.VolunteerId, userId ?? Guid.Empty, true);
        
        var result = await _mediator.Send(command, cancellationToken);
        if (result == null) return Conflict(ApiResponse.Fail(System.Net.HttpStatusCode.Conflict, "Volunteer is already assigned or invited."));
        return Ok(ApiResponse.Success(_mapper.Map<TaskAssignmentResponse>(result)));
    }

    [HttpPost("api/relief-requests/{requestId}/tasks/{taskId}/assignments/assign")]
    [Authorize(Roles = "Coordinator,Admin")]
    public async Task<IActionResult> AssignVolunteer(Guid requestId, Guid taskId, [FromBody] AssignTaskApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new AssignTaskCommand(taskId, request.VolunteerId, userId ?? Guid.Empty, false);
        
        var result = await _mediator.Send(command, cancellationToken);
        if (result == null) return Conflict(ApiResponse.Fail(System.Net.HttpStatusCode.Conflict, "Volunteer is already assigned or invited."));
        return Ok(ApiResponse.Success(_mapper.Map<TaskAssignmentResponse>(result)));
    }

    // ==========================================
    // 3. MY TASKS (Volunteer)
    // ==========================================
    [HttpGet("api/me/tasks")]
    [Authorize]
    public async Task<IActionResult> GetMyTasks(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new GetMyReliefTasksQuery(userId.Value), cancellationToken);
        return Ok(ApiResponse.Success(_mapper.Map<IEnumerable<ReliefTaskResponse>>(result)));
    }

    [HttpGet("api/me/tasks/{taskId}")]
    [Authorize]
    public async Task<IActionResult> GetMyTask(Guid taskId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new GetMyReliefTaskByIdQuery(userId.Value, taskId), cancellationToken);
        if (result == null) return NotFound(ApiResponse.Fail(System.Net.HttpStatusCode.NotFound, "Task not found"));
        return Ok(ApiResponse.Success(_mapper.Map<ReliefTaskResponse>(result)));
    }
}
