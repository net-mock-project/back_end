using RescueHub.Infrastructure.SqlServer.Models;

public class RoleDataModel
{
    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<UserDataModel> Users { get; set; } = new List<UserDataModel>();
}