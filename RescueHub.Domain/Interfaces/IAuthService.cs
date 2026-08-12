using RescueHub.Domain.Enums; 
using System;
using System.Threading.Tasks;

namespace RescueHub.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SendOtpAsync(string fullName, DateTime dateOfBirth, string email, string phoneNumber, Gender gender, string password, string address);
        Task<bool> ResendOtpAsync(string email);
        Task<bool> RegisterAsync(string email, string otpCode);
    }
}