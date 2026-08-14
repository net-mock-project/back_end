namespace RescueHub.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(
        Guid userId,
        string email,
        Guid roleId);
}