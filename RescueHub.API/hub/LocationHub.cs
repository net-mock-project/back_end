using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RescueHub.Application.Features.Donations.Commands;
using RescueHub.Application.Features.Users.Commands;
using RescueHub.Domain.Entities;
using System.Security.Claims;

namespace RescueHub.API.hub
{
    [Authorize]
    public class LocationHub : Hub
    {
        private readonly ISender _sender;

        public LocationHub(ISender sender)
        {
            _sender = sender;
        }
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

            // Gửi vị trí mới
            await Clients.All
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
}
