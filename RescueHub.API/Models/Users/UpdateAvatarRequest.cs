namespace RescueHub.API.Models.Users;

public class UpdateAvatarRequest
{
    public IFormFile Avatar { get; set; } = null!;
}