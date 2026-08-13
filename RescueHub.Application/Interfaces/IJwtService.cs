namespace RescueHub.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(
        Guid userId,
        string email,
        int roleId);
}