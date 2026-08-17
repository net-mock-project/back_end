namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class RoleDataModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserDataModel> Users { get; set; }
            = new List<UserDataModel>();
    }
}