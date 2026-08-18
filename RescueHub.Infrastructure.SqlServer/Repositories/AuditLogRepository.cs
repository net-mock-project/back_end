using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuditLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<AuditLog>> GetPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.AuditLogs
                .AsNoTracking()
                .AsQueryable();

            query = ApplySearch(query, criteria.Search);
            query = ApplyFilters(query, criteria.Filters);
            query = ApplySorting(query, criteria.SortBy, criteria.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);

            var dataModels = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync(cancellationToken);

            var items = dataModels
                .Select(MapToDomain)
                .ToList();

            return new PagedResult<AuditLog>(items, totalCount);
        }

        public async Task<AuditLog?> GetByIdAsync(
            Guid auditLogId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.AuditLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == auditLogId,
                    cancellationToken);

            return dataModel == null
                ? null
                : MapToDomain(dataModel);
        }

        public Task<AuditLog> CreateAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dataModel = MapToDataModel(auditLog);
            _dbContext.AuditLogs.Add(dataModel);

            return Task.FromResult(auditLog);
        }

        private static IQueryable<AuditLogDataModel> ApplySearch(
            IQueryable<AuditLogDataModel> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();

            return query.Where(x =>
                x.Action.Contains(search) ||
                x.EntityName.Contains(search) ||
                (x.OldValue != null && x.OldValue.Contains(search)) ||
                (x.NewValue != null && x.NewValue.Contains(search)));
        }

        private static IQueryable<AuditLogDataModel> ApplyFilters(
            IQueryable<AuditLogDataModel> query,
            IReadOnlyList<FilterCriteria> filters)
        {
            foreach (var filter in filters)
            {
                var field = filter.Field.Trim();

                if (field.Equals("id", StringComparison.OrdinalIgnoreCase))
                    query = ApplyGuidFilter(query, filter, x => x.Id);
                else if (field.Equals("userId", StringComparison.OrdinalIgnoreCase))
                    query = ApplyGuidFilter(query, filter, x => x.UserId);
                else if (field.Equals("entityId", StringComparison.OrdinalIgnoreCase))
                    query = ApplyGuidFilter(query, filter, x => x.EntityId);
                else if (field.Equals("action", StringComparison.OrdinalIgnoreCase))
                    query = ApplyStringFilter(query, filter, x => x.Action);
                else if (field.Equals("entityName", StringComparison.OrdinalIgnoreCase))
                    query = ApplyStringFilter(query, filter, x => x.EntityName);
                else if (field.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                    query = ApplyDateTimeFilter(query, filter, x => x.CreatedAt);
            }

            return query;
        }

        private static IQueryable<AuditLogDataModel> ApplyGuidFilter(
            IQueryable<AuditLogDataModel> query,
            FilterCriteria filter,
            System.Linq.Expressions.Expression<Func<AuditLogDataModel, Guid>> selector)
        {
            if (!Guid.TryParse(filter.Value, out var value))
                return query.Where(_ => false);

            var parameter = selector.Parameters[0];
            var property = selector.Body;
            var constant = System.Linq.Expressions.Expression.Constant(value);
            var body = filter.Operator switch
            {
                FilterOperator.Equals => System.Linq.Expressions.Expression.Equal(property, constant),
                FilterOperator.NotEquals => System.Linq.Expressions.Expression.NotEqual(property, constant),
                _ => null!
            };

            if (body == null) return query.Where(_ => false);

            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<AuditLogDataModel, bool>>(body, parameter));
        }

        private static IQueryable<AuditLogDataModel> ApplyStringFilter(
            IQueryable<AuditLogDataModel> query,
            FilterCriteria filter,
            System.Linq.Expressions.Expression<Func<AuditLogDataModel, string>> selector)
        {
            var value = filter.Value?.Trim() ?? string.Empty;
            var parameter = selector.Parameters[0];
            var property = selector.Body;
            var constant = System.Linq.Expressions.Expression.Constant(value);

            System.Linq.Expressions.Expression body = filter.Operator switch
            {
                FilterOperator.Equals => System.Linq.Expressions.Expression.Equal(property, constant),
                FilterOperator.NotEquals => System.Linq.Expressions.Expression.NotEqual(property, constant),
                FilterOperator.Contains => System.Linq.Expressions.Expression.Call(
                    property,
                    nameof(string.Contains),
                    Type.EmptyTypes,
                    constant),
                _ => null!
            };

            if (body == null) return query.Where(_ => false);

            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<AuditLogDataModel, bool>>(body, parameter));
        }

        private static IQueryable<AuditLogDataModel> ApplyDateTimeFilter(
            IQueryable<AuditLogDataModel> query,
            FilterCriteria filter,
            System.Linq.Expressions.Expression<Func<AuditLogDataModel, DateTime>> selector)
        {
            if (!DateTime.TryParse(filter.Value, out var value))
                return query.Where(_ => false);

            var parameter = selector.Parameters[0];
            var property = selector.Body;
            var constant = System.Linq.Expressions.Expression.Constant(value);

            var body = filter.Operator switch
            {
                FilterOperator.Equals => System.Linq.Expressions.Expression.Equal(property, constant),
                FilterOperator.NotEquals => System.Linq.Expressions.Expression.NotEqual(property, constant),
                FilterOperator.GreaterThan => System.Linq.Expressions.Expression.GreaterThan(property, constant),
                FilterOperator.GreaterThanOrEqual => System.Linq.Expressions.Expression.GreaterThanOrEqual(property, constant),
                FilterOperator.LessThan => System.Linq.Expressions.Expression.LessThan(property, constant),
                FilterOperator.LessThanOrEqual => System.Linq.Expressions.Expression.LessThanOrEqual(property, constant),
                _ => null
            };

            if (body == null) return query.Where(_ => false);

            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<AuditLogDataModel, bool>>(body, parameter));
        }

        private static IQueryable<AuditLogDataModel> ApplySorting(
            IQueryable<AuditLogDataModel> query,
            string? sortBy,
            SortDirection sortDirection)
        {
            var field = sortBy?.Trim();

            if (string.IsNullOrWhiteSpace(field))
                return query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id);

            var descending = sortDirection == SortDirection.Desc;

            return field.ToLowerInvariant() switch
            {
                "id" => descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
                "userid" => descending ? query.OrderByDescending(x => x.UserId) : query.OrderBy(x => x.UserId),
                "action" => descending ? query.OrderByDescending(x => x.Action) : query.OrderBy(x => x.Action),
                "entityname" => descending ? query.OrderByDescending(x => x.EntityName) : query.OrderBy(x => x.EntityName),
                "entityid" => descending ? query.OrderByDescending(x => x.EntityId) : query.OrderBy(x => x.EntityId),
                "createdat" => descending
                    ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
                _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
            };
        }

        private static AuditLog MapToDomain(AuditLogDataModel dataModel)
        {
            return new AuditLog(
                dataModel.Id,
                dataModel.UserId,
                dataModel.Action,
                dataModel.EntityName,
                dataModel.EntityId,
                dataModel.OldValue,
                dataModel.NewValue,
                dataModel.CreatedAt);
        }

        private static AuditLogDataModel MapToDataModel(AuditLog auditLog)
        {
            return new AuditLogDataModel
            {
                Id = auditLog.Id,
                UserId = auditLog.UserId,
                Action = auditLog.Action,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                CreatedAt = auditLog.CreatedAt
            };
        }
    }
}
