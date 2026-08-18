using Mapster;
using MediatR;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Domain.Interfaces.AuditLogs;

namespace RescueHub.Application.Features.AuditLogs.Queries
{
    public record GetAuditLogByIdQuery(
        Guid LogId
    ) : IRequest<AuditLogDto?>;

    public class GetAuditLogByIdQueryHandler
        : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
    {
        private readonly IAuditLogService _auditLogService;

        public GetAuditLogByIdQueryHandler(
            IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<AuditLogDto?> Handle(
            GetAuditLogByIdQuery request,
            CancellationToken cancellationToken)
        {
            var auditLog =
                await _auditLogService.GetByIdAsync(
                    request.LogId,
                    cancellationToken);

            return auditLog?.Adapt<AuditLogDto>();
        }
    }
}
