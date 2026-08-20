using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RescueHub.Application.Features.Users.Commands;
using System.Security.Claims;

namespace RescueHub.API.Hubs;

[Authorize]
public class LocationHub : Hub
{
    private const string CoordinatorGroup = "coordinators";

    private readonly ISender _sender;

    public LocationHub(ISender sender)
    {
        _sender = sender;
    }

    // Khi client kết nối vào SignalR Hub
    public override async Task OnConnectedAsync()
    {
        var role = Context.User?
            .FindFirstValue(ClaimTypes.Role);

        // Coordinator sẽ được đưa vào group
        // để nhận vị trí realtime
        if (role == "Coordinator")
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                CoordinatorGroup);
        }

        await base.OnConnectedAsync();
    }

    // Client gửi vị trí mới lên server
    public async Task UpdateLocation(
        double latitude,
        double longitude)
    {
        // Lấy UserId từ JWT
        var userIdValue = Context.User?
            .FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
            userIdValue,
            out var userId))
        {
            throw new HubException(
                "Unauthorized.");
        }

        // Gọi Application layer
        var command = new UpdateLocationCommand(
            userId,
            latitude,
            longitude);

        var success = await _sender.Send(
            command,
            Context.ConnectionAborted);

        if (!success)
        {
            throw new HubException(
                "User not found.");
        }

        // Gửi vị trí mới cho Coordinator
        await Clients
            .Group(CoordinatorGroup)
            .SendAsync(
                "UserLocationUpdated",
                new
                {
                    UserId = userId,
                    Latitude = latitude,
                    Longitude = longitude,
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                Context.ConnectionAborted);
    }
}