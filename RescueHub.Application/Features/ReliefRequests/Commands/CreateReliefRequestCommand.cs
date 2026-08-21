using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.ReliefRequests;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Notifications;
using RescueHub.Domain.Interfaces.ReliefRequests;
using RescueHub.Domain.Interfaces.Users;
using System.Text.Json;

namespace RescueHub.Application.Features.ReliefRequests.Commands
{
    public record CreateReliefRequestCommand(
        Guid RequesterId,
        double Longitude,
        double Latitude,
        string Title,
        string Description,
        string? ReliefImageUrl,
        string? RequestedResource,
        int UrgencyLevel,
        int EstimatedAffectedPeople,
        decimal? EstimatedAffectedRadiusKm) : IRequest<ReliefRequestDto>;

    public class CreateReliefRequestCommandHandler : IRequestHandler<CreateReliefRequestCommand, ReliefRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReliefRequestService _service;
        private readonly IUserService _userService;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;

        public CreateReliefRequestCommandHandler(
            IUnitOfWork unitOfWork, 
            IReliefRequestService service,
            IAuditLogService auditLogService,
            INotificationService notificationService,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _service = service;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<ReliefRequestDto> Handle(CreateReliefRequestCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var location = new RescueHub.Domain.Common.Enums.GeoLocation(request.Latitude, request.Longitude);
                var entity = await _service.CreateReliefRequestAsync(
                    request.RequesterId,
                    location,
                    request.Title,
                    request.Description,
                    request.ReliefImageUrl,
                    request.RequestedResource,
                    request.UrgencyLevel,
                    request.EstimatedAffectedPeople,
                    request.EstimatedAffectedRadiusKm,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _auditLogService.CreateAsync(
                    new AuditLog(
                        request.RequesterId,
                        "Create",
                        nameof(ReliefRequest),
                        entity.Id,
                        newValue: JsonSerializer.Serialize(new
                        {
                            entity.Id,
                            entity.Title,
                            entity.Description,
                            entity.UrgencyLevel,
                            entity.EstimatedAffectedPeople,
                            entity.EstimatedAffectedRadiusKm
                        })),
                    cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (request.EstimatedAffectedRadiusKm.HasValue)
                {
                    var affectedUsers =
                        await _userService.GetUsersWithinRangeAsync(
                            request.Latitude,
                            request.Longitude,
                            (double)request.EstimatedAffectedRadiusKm.Value * 1000,
                            cancellationToken);

                    foreach (var user in affectedUsers)
                    {
                        var notification = new Notification(
                            user.Id,
                            "New Relief Request",
                            $"A new relief request has been created: {entity.Title}",
                            NotificationType.ReliefRequest,
                            $"/relief-requests/{entity.Id}"
                        );

                        await _notificationService.CreateAsync(
                            notification,
                            cancellationToken);
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                return entity.Adapt<ReliefRequestDto>();
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
