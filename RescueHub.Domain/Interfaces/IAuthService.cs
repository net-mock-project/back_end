// RescueHub.Domain/Services/IAuthService.cs
using RescueHub.Domain.Entities.RegisterDTOs; // hoặc namespace chứa DTO của bạn

namespace RescueHub.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string> VerifyAndRegisterAsync(RegisterDto dto);
        Task<bool> SendOtpAsync(string phone); 
    }
}