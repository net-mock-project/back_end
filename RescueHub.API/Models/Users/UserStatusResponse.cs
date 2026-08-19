using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Users
{
    public class UserStatusResponse
    {
        public Guid Id { get; set; }

        public UserStatus Status { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}