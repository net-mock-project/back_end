namespace RescueHub.Application.Contracts.Auth;

public record LoginResultDto(
    string AccessToken,
    Guid UserId,
    string Email,
    Guid RoleId,
    string RoleName
);