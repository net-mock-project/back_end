using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;

namespace RescueHub.Domain.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public Task<PagedResult<AuditLog>> GetPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            return _auditLogRepository.GetPagedAsync(criteria, cancellationToken);
        }

        public Task<AuditLog?> GetByIdAsync(
            Guid auditLogId,
            CancellationToken cancellationToken)
        {
            return _auditLogRepository.GetByIdAsync(auditLogId, cancellationToken);
        }

        public async Task<AuditLog> CreateAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken)
        {
            return await _auditLogRepository.CreateAsync(
                auditLog,
                cancellationToken);
        }
    }
}
