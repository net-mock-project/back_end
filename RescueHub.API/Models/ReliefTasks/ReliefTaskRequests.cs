namespace RescueHub.API.Models.ReliefTasks;

public class CreateReliefTaskApiRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int RequiredVolunteers { get; set; }
    public int Priority { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<Guid>? TaskSkills { get; set; }
}

public class UpdateReliefTaskApiRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int RequiredVolunteers { get; set; }
    public int Priority { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<Guid>? TaskSkills { get; set; }
}

public class AssignTaskApiRequest
{
    public Guid VolunteerId { get; set; }
}
