using Mapster;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Application.Contracts.Notifications;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Application.Contracts.Donation;

using RescueHub.Application.Contracts.ReliefRequests;

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
            config.NewConfig<User, UserListDto>();
            config.NewConfig<User, UserDetailDto>();
            config.NewConfig<User, UserStatusDto>();
            config.NewConfig<AuditLog, AuditLogDto>();

            config.NewConfig<Notification, NotificationDto>()
                .Map(dest => dest.Type, src => src.Type.ToString());

            config.NewConfig<Volunteer, VolunteerProfileDto>()
                .Map(dest => dest.Id, src => src.VolunteerId)
                .Map(dest => dest.Skills, src => src.Skills)
                .Map(dest => dest.Latitude, src => src.Location != null ? src.Location.Latitude : (double?)null)
                .Map(dest => dest.Longitude, src => src.Location != null ? src.Location.Longitude : (double?)null);
            config.NewConfig<VolunteerSkill, VolunteerSkillDto>();


            config.NewConfig<Donation, DonationDto>()
                // 1. Map Id sang DonationId
                .Map(dest => dest.DonationId, src => src.Id)

                // 2. Map DonationDate từ Entity sang DTO
                .Map(dest => dest.DonationDate, src => src.DonationDate)

                // 3. Map Status
                .Map(dest => dest.Status, src => src.Status)

                // 4. Lấy tên Kho (Warehouse Name) từ giao dịch đầu tiên
                .Map(dest => dest.WarehouseName, src => src.DonationTransactions
                    .FirstOrDefault() != null
                    && src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions != null
                    && src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions!.WarehouseInventories != null
                    && src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions!.WarehouseInventories!.Warehouses != null
                        ? src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions!.WarehouseInventories!.Warehouses!.Name
                        : string.Empty)

                // 5. Map danh sách các vật phẩm (Items) từ các transaction liên quan
                .Map(dest => dest.Items, src => src.DonationTransactions.Select(dt => new DonationItemRequest
                {
                    SupplyName = dt.WarehouseTransactions != null
                                 && dt.WarehouseTransactions.WarehouseInventories != null
                                 && dt.WarehouseTransactions.WarehouseInventories.Supplys != null
                        ? dt.WarehouseTransactions.WarehouseInventories.Supplys.Name
                        : string.Empty,

                    Quantity = dt.WarehouseTransactions != null
                        ? dt.WarehouseTransactions.Quantity
                        : 0,

                    Unit = dt.WarehouseTransactions != null
                           && dt.WarehouseTransactions.WarehouseInventories != null
                           && dt.WarehouseTransactions.WarehouseInventories.Supplys != null
                        ? dt.WarehouseTransactions.WarehouseInventories.Supplys.Unit
                        : string.Empty
                }).ToList())

                // 6. Lấy thông tin Donor
                .Map(dest => dest.DonatorName, src => src.Donator != null ? src.Donator.FullName : string.Empty)
                .Map(dest => dest.DonatorPhone, src => src.Donator != null ? src.Donator.Phone : string.Empty);

            config.NewConfig<ReliefRequest, ReliefRequestDto>()
                .Map(dest => dest.Longitude, src => src.Location != null ? src.Location.Longitude : 0)
                .Map(dest => dest.Latitude, src => src.Location != null ? src.Location.Latitude : 0);

            config.NewConfig<ReliefTask, RescueHub.Application.Contracts.ReliefTasks.ReliefTaskDto>()
                .Map(dest => dest.Latitude, src => src.Location != null ? src.Location.Latitude : (double?)null)
                .Map(dest => dest.Longitude, src => src.Location != null ? src.Location.Longitude : (double?)null);

            config.NewConfig<TaskAssignment, RescueHub.Application.Contracts.TaskAssignments.TaskAssignmentDto>();
            config.NewConfig<VolunteerEngagement, RescueHub.Application.Contracts.VolunteerEngagements.VolunteerEngagementDto>();
        }
    }
}