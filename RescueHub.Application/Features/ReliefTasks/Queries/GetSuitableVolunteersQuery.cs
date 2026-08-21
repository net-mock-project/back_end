using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Interfaces.Volunteers;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Entities;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Queries;

public record GetSuitableVolunteersQuery(Guid RequestId, Guid TaskId) : IRequest<List<VolunteerMatchDto>>;

public class GetSuitableVolunteersQueryHandler : IRequestHandler<GetSuitableVolunteersQuery, List<VolunteerMatchDto>>
{
    private readonly IReliefTaskRepository _taskRepository;
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVolunteerEngagementRepository _engagementRepository;

    public GetSuitableVolunteersQueryHandler(
        IReliefTaskRepository taskRepository,
        IVolunteerRepository volunteerRepository,
        IUserRepository userRepository,
        IVolunteerEngagementRepository engagementRepository)
    {
        _taskRepository = taskRepository;
        _volunteerRepository = volunteerRepository;
        _userRepository = userRepository;
        _engagementRepository = engagementRepository;
    }

    public async Task<List<VolunteerMatchDto>> Handle(GetSuitableVolunteersQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null || task.RequestId != request.RequestId) return new List<VolunteerMatchDto>();

        // Get all approved volunteers
        var allVolunteers = await _volunteerRepository.GetApprovedPagedAsync(new RescueHub.Domain.Common.Querying.QueryCriteria { PageSize = 1000 }, cancellationToken);
        
        var matches = new List<VolunteerMatchDto>();

        foreach (var volunteer in allVolunteers.Items)
        {
            var user = await _userRepository.GetByIdAsync(volunteer.VolunteerId, cancellationToken);
            if (user == null) continue;

            // 1. Availability Score (40 pts)
            int availabilityScore = 0;
            var engagement = await _engagementRepository.GetByVolunteerAndRequestAsync(volunteer.VolunteerId, task.RequestId, cancellationToken);
            if (engagement != null && engagement.Status == VolunteerEngagementStatus.Active)
            {
                availabilityScore = 40;
            }

            // 2. Skill Score (40 pts)
            int skillScore = 0;
            var matchedSkills = new List<Guid>();
            if (task.TaskSkills != null && task.TaskSkills.Any())
            {
                var volunteerSkillIds = volunteer.Skills.Select(s => s.SkillId).ToList();
                var matchCount = task.TaskSkills.Count(ts => volunteerSkillIds.Contains(ts));
                matchedSkills = task.TaskSkills.Where(ts => volunteerSkillIds.Contains(ts)).ToList();
                skillScore = (int)((double)matchCount / task.TaskSkills.Count * 40);
            }
            else
            {
                skillScore = 40; // If task requires no skills, full points
            }

            // 3. Distance Score (20 pts)
            int distanceScore = 0;
            if (task.Location != null && user.Location != null)
            {
                double distanceKm = CalculateDistance(task.Location.Latitude, task.Location.Longitude, user.Location.Latitude, user.Location.Longitude);
                if (distanceKm < 50)
                {
                    distanceScore = (int)(20 - (distanceKm / 50.0) * 20);
                }
            }
            else
            {
                // Default if no location available
                distanceScore = 0;
            }

            int totalScore = availabilityScore + skillScore + distanceScore;

            matches.Add(new VolunteerMatchDto(
                volunteer.VolunteerId,
                user.FullName,
                user.ProfileUrl ?? "",
                distanceScore,
                skillScore,
                availabilityScore,
                totalScore,
                matchedSkills
            ));
        }

        return matches.OrderByDescending(m => m.TotalScore).Take(5).ToList();
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
        var dLon = Deg2Rad(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
            ;
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }

    private double Deg2Rad(double deg)
    {
        return deg * (Math.PI / 180);
    }
}
