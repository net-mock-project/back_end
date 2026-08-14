namespace RescueHub.Application.Contracts.Authentication;

public record LoginResultDto(
    string AccessToken,
    Guid UserId,
    string Email,
    Guid RoleId
);