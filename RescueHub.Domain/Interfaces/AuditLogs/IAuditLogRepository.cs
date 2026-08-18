using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.AuditLogs
{
    public interface IAuditLogRepository
    {
        Task<PagedResult<AuditLog>> GetPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        Task<AuditLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<AuditLog> CreateAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken);
    }
}
