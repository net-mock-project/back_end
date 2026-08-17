namespace RescueHub.API.Models.Auth;

public class LoginResponse
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public Guid RoleId { get; set; }

    public string Message { get; set; } = string.Empty;
}
