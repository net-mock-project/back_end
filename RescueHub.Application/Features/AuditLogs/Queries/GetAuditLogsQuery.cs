using Mapster;
using MediatR;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Interfaces.AuditLogs;

namespace RescueHub.Application.Features.AuditLogs.Queries
{
    public record GetAuditLogsQuery(
        QueryCriteria Criteria
    ) : IRequest<PaginationResponse<AuditLogDto>>;

    public class GetAuditLogsQueryHandler
        : IRequestHandler<
            GetAuditLogsQuery,
            PaginationResponse<AuditLogDto>>
    {
        private readonly IAuditLogService _auditLogService;

        public GetAuditLogsQueryHandler(
            IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<PaginationResponse<AuditLogDto>> Handle(
            GetAuditLogsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _auditLogService.GetPagedAsync(
                request.Criteria,
                cancellationToken);

            var items = result.Items
                .Select(x => x.Adapt<AuditLogDto>())
                .ToList();

            return new PaginationResponse<AuditLogDto>(
                items,
                result.TotalCount,
                request.Criteria.PageNumber,
                request.Criteria.PageSize);
        }
    }
}
