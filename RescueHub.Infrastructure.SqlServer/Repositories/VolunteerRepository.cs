using Microsoft.EntityFrameworkCore;
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
            }
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
                skills);
        }
    }
}