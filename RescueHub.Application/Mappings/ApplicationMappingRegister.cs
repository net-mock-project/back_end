using Mapster;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Application.Contracts.Notifications;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Application.Contracts.Donation;

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

            config.NewConfig<Donation, DonationDto>()
                // 1. Map Id sang DonationId
                .Map(dest => dest.DonationId, src => src.Id)

                // 2. Map DonationDate từ Entity sang DTO
                .Map(dest => dest.DonationDate, src => src.DonationDate)

                // 3. Ép kiểu Enum Status sang string
                .Map(dest => dest.Status, src => src.Status.ToString())

                // 4. Lấy tên Supply từ bảng liên quan
                .Map(dest => dest.SupplyName, src => src.DonationTransactions
                    .FirstOrDefault() != null ? src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions.WarehouseInventories.Supplys.Name : string.Empty)

                // 5. Lấy Unit từ Supply
                .Map(dest => dest.Unit, src => src.DonationTransactions
                    .FirstOrDefault() != null ? src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions.WarehouseInventories.Supplys.Unit : string.Empty)

                // 6. Lấy tên Kho (Warehouse Name)
                .Map(dest => dest.WarehouseName, src => src.DonationTransactions
                    .FirstOrDefault() != null ? src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions.WarehouseInventories.Warehouses.Name : string.Empty)

                // 7. Lấy Quantity từ Transaction
                .Map(dest => dest.Quantity, src => src.DonationTransactions
                    .FirstOrDefault() != null ? src.DonationTransactions.FirstOrDefault()!.WarehouseTransactions.WarehouseInventories.Quantity : 0)

                // 8. Lấy thông tin Donor
                .Map(dest => dest.DonatorName, src => src.Donator != null ? src.Donator.FullName : string.Empty)
                .Map(dest => dest.DonatorPhone, src => src.Donator != null ? src.Donator.Phone : string.Empty);
        }
    }
}