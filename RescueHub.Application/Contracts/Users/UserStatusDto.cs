using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.Users
{
    public class UserStatusDto
    {
        public Guid Id { get; set; }

        public UserStatus Status { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}