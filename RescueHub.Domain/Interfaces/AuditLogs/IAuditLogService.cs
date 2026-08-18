using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.AuditLogs
{
    public interface IAuditLogService
    {
        Task<PagedResult<AuditLog>> GetPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        Task<AuditLog?> GetByIdAsync(
            Guid auditLogId,
            CancellationToken cancellationToken);

        Task<AuditLog> CreateAsync(
           AuditLog auditLog,
           CancellationToken cancellationToken);
    }
}
