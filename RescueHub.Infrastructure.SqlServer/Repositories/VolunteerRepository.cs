using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Volunteers;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VolunteerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Volunteer?> GetByIdAsync(
            Guid volunteerId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Volunteers
                .AsNoTracking()
                .Include(v => v.User)
                .Include(v => v.VolunteerSkills)
                    .ThenInclude(vs => vs.Skill)
                .FirstOrDefaultAsync(v => v.Id == volunteerId, cancellationToken);

            return dataModel == null ? null : MapToDomain(dataModel);
        }

        public async Task AddAsync(
            Volunteer volunteer,
            CancellationToken cancellationToken)
        {
            var dataModel = new VolunteerDataModel
            {
                Id = volunteer.VolunteerId,
                ExperienceYears = volunteer.ExperienceYears,
                ApprovalStatus = volunteer.ApprovalStatus,
                CVUrl = volunteer.CVUrl,
                ApprovedBy = volunteer.ApprovedBy,
                ApprovedAt = volunteer.ApprovedAt,
                CreatedAt = volunteer.CreatedAt,
                UpdatedAt = volunteer.UpdatedAt,
                DeletedAt = volunteer.DeletedAt,
                VolunteerSkills = volunteer.Skills.Select(s => new VolunteerSkillDataModel
                {
                    VolunteerId = volunteer.VolunteerId,
                    SkillId = s.SkillId,
                    Level = s.Level
                }).ToList()
            };

            await _dbContext.Volunteers.AddAsync(dataModel, cancellationToken);
        }

        public async Task UpdateAsync(
            Volunteer volunteer,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Volunteers
                .Include(v => v.VolunteerSkills)
                .FirstOrDefaultAsync(v => v.Id == volunteer.VolunteerId, cancellationToken);

            if (dataModel != null)
            {
                dataModel.ExperienceYears = volunteer.ExperienceYears;
                dataModel.ApprovalStatus = volunteer.ApprovalStatus;
                dataModel.CVUrl = volunteer.CVUrl;
                dataModel.ApprovedBy = volunteer.ApprovedBy;
                dataModel.ApprovedAt = volunteer.ApprovedAt;
                dataModel.UpdatedAt = volunteer.UpdatedAt;
                dataModel.DeletedAt = volunteer.DeletedAt;

                _dbContext.VolunteerSkills.RemoveRange(dataModel.VolunteerSkills);
                dataModel.VolunteerSkills = volunteer.Skills.Select(s => new VolunteerSkillDataModel
                {
                    VolunteerId = volunteer.VolunteerId,
                    SkillId = s.SkillId,
                    Level = s.Level
                }).ToList();
            }
        }

        public async Task<PagedResult<Volunteer>> GetPendingPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Volunteers
                .AsNoTracking()
                .Where(x => x.ApprovalStatus == VolunteerApprovalStatus.Pending && x.DeletedAt == null);

            query = ApplySearch(query, criteria.Search);
            query = ApplyFilters(query, criteria.Filters);
            query = ApplySorting(query, criteria.SortBy, criteria.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);

            var dataModels = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .Include(v => v.User)
                .Include(v => v.VolunteerSkills)
                    .ThenInclude(vs => vs.Skill)
                .ToListAsync(cancellationToken);

            var items = dataModels
                .Select(MapToDomain)
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();

            return new PagedResult<Volunteer>(items, totalCount);
        }

        public async Task<PagedResult<Volunteer>> GetApprovedPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Volunteers
                .AsNoTracking()
                .Where(x => x.ApprovalStatus == VolunteerApprovalStatus.Approved && x.DeletedAt == null);

            query = ApplySearch(query, criteria.Search);
            query = ApplyFilters(query, criteria.Filters);
            query = ApplySorting(query, criteria.SortBy, criteria.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);

            var dataModels = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .Include(v => v.User)
                .Include(v => v.VolunteerSkills)
                    .ThenInclude(vs => vs.Skill)
                .ToListAsync(cancellationToken);

            var items = dataModels
                .Select(MapToDomain)
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();

            return new PagedResult<Volunteer>(items, totalCount);
        }

        private static IQueryable<VolunteerDataModel> ApplySearch(
            IQueryable<VolunteerDataModel> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();

            return query.Where(x =>
                (x.User != null && x.User.FullName.Contains(search)) ||
                (x.User != null && x.User.Email.Contains(search)) ||
                (x.CVUrl != null && x.CVUrl.Contains(search)));
        }

        private static IQueryable<VolunteerDataModel> ApplyFilters(
            IQueryable<VolunteerDataModel> query,
            IReadOnlyList<FilterCriteria> filters)
        {
            foreach (var filter in filters)
            {
                var field = filter.Field.Trim();

                if (field.Equals("id", StringComparison.OrdinalIgnoreCase))
                    query = ApplyGuidFilter(query, filter, x => x.Id);
                else if (field.Equals("experienceYears", StringComparison.OrdinalIgnoreCase))
                    query = ApplyIntFilter(query, filter, x => x.ExperienceYears);
                else if (field.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                    query = ApplyDateTimeFilter(query, filter, x => x.CreatedAt);
                else if (field.Equals("province", StringComparison.OrdinalIgnoreCase))
                {
                    var provinceVal = filter.Value?.Trim() ?? string.Empty;
                    query = query.Where(x => x.User != null && x.User.Province != null && x.User.Province.Contains(provinceVal));
                }
                else if (field.Equals("skillId", StringComparison.OrdinalIgnoreCase))
                {
                    if (Guid.TryParse(filter.Value, out var skillId))
                    {
                        query = query.Where(x => x.VolunteerSkills.Any(s => s.SkillId == skillId));
                    }
                }
            }

            return query;
        }

        private static IQueryable<VolunteerDataModel> ApplyGuidFilter(
            IQueryable<VolunteerDataModel> query,
            FilterCriteria filter,
            System.Linq.Expressions.Expression<Func<VolunteerDataModel, Guid>> selector)
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

            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<VolunteerDataModel, bool>>(body, parameter));
        }

        private static IQueryable<VolunteerDataModel> ApplyIntFilter(
            IQueryable<VolunteerDataModel> query,
            FilterCriteria filter,
            System.Linq.Expressions.Expression<Func<VolunteerDataModel, int>> selector)
        {
            if (!int.TryParse(filter.Value, out var value))
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

            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<VolunteerDataModel, bool>>(body, parameter));
        }

        private static IQueryable<VolunteerDataModel> ApplyDateTimeFilter(
            IQueryable<VolunteerDataModel> query,
            FilterCriteria filter,
            System.Linq.Expressions.Expression<Func<VolunteerDataModel, DateTime>> selector)
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

            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<VolunteerDataModel, bool>>(body, parameter));
        }

        private static IQueryable<VolunteerDataModel> ApplySorting(
            IQueryable<VolunteerDataModel> query,
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
                "experienceyears" => descending ? query.OrderByDescending(x => x.ExperienceYears) : query.OrderBy(x => x.ExperienceYears),
                "createdat" => descending
                    ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
                _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
            };
        }

        private Volunteer? MapToDomain(VolunteerDataModel? dataModel)
        {
            if (dataModel == null)
                return null;

            var skills = dataModel.VolunteerSkills?.Select(vs =>
                new VolunteerSkill(
                    vs.VolunteerId,
                    vs.SkillId,
                    vs.Level,
                    vs.Skill?.Name)
            ).ToList();

            return new Volunteer(
                dataModel.Id,
                dataModel.ExperienceYears,
                dataModel.ApprovalStatus,
                dataModel.CVUrl,
                dataModel.ApprovedBy,
                dataModel.ApprovedAt,
                dataModel.CreatedAt,
                dataModel.UpdatedAt,
                dataModel.DeletedAt,
                skills,
                fullName: dataModel.User?.FullName,
                email: dataModel.User?.Email,
                phone: dataModel.User?.Phone,
                profileUrl: dataModel.User?.ProfileUrl,
                province: dataModel.User?.Province);
        }
    }
}