namespace RescueHub.API.Models.Authentication;

public class LoginResponse
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}