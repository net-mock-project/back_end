using Mapster;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Application.Contracts.Notifications;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.ReadModels.Users;

namespace RescueHub.Application.Mappings
{
    public class ApplicationMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FilterRequest, FilterCriteria>();
            config.NewConfig<QueryRequest, QueryCriteria>()
                .Map(dest => dest.Filters, src => src.Filters);

            // Map Domain Entity sang DTO
            config.NewConfig<User, UserProfileDto>();
            config.NewConfig<UserProfileItem, UserProfileDto>();
            config.NewConfig<UserListItem, UserListDto>();
            config.NewConfig<UserDetailItem, UserDetailDto>();
            config.NewConfig<User, UserStatusDto>();
            config.NewConfig<AuditLog, AuditLogDto>();
            config.NewConfig<Notification, NotificationDto>()
                .Map(dest => dest.Type, src => src.Type.ToString());
        }
    }
}