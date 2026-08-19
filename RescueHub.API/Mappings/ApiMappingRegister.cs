using Mapster;
using RescueHub.API.Models.AuditLogs;
using RescueHub.API.Models.Auth;
using RescueHub.API.Models.Notifications;
using RescueHub.API.Models.Users;
using RescueHub.API.Models.Donation;
using RescueHub.API.Models.Volunteers;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Application.Contracts.Notifications;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Application.Contracts.Donation;
using RescueHub.Application.Contracts.Volunteers;
using RescueHub.Application.Features.Auth.Commands;
using RescueHub.Application.Features.Users.Commands;
using RescueHub.Application.Features.Donations.Commands;
using RescueHub.Application.Features.Volunteers.Commands;
using RescueHub.Application.Contracts.Querying;

namespace RescueHub.API.Mappings
{
    // Ánh xạ giữa model của tầng API (Request/Response) và Command/Query/DTO của tầng Application.
    public class ApiMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Request -> Command
            config.NewConfig<UpdateProfileRequest, UpdateProfileCommand>()
                .MapWith(request => new UpdateProfileCommand(
                    Guid.Empty,
                    request.FullName,
                    request.Phone,
                    request.DateOfBirth,
                    request.Gender));

            config.NewConfig<SendOtpRequest, SendOtpCommand>()
                .MapWith(src => new SendOtpCommand(
                    src.FullName,
                    src.DateOfBirth,
                    src.Email,
                    src.Phone,
                    src.Gender,
                    src.Password,
                    src.Address
                ));

            config.NewConfig<UpdateDonationRequest, UpdateDonationCommand>()
                .MapWith(src => new UpdateDonationCommand(
                    Guid.Empty,
                    Guid.Empty,
                    src.Items != null
                        ? src.Items.Select(i => i == null ? null : new RescueHub.Application.Contracts.Donation.DonationItemRequest
                        {
                            SupplyName = i.SupplyName,
                            Quantity = i.Quantity,
                            Unit = i.Unit
                        }).ToList()
                        : null,
                    src.DonationDate
                ));

            config.NewConfig<CreateDonationRequest, CreateDonationCommand>()
                .MapWith(src => new CreateDonationCommand(
                    Guid.Empty,
                    src.Items != null
                        ? src.Items.Select(i => i == null ? null : new RescueHub.Application.Contracts.Donation.DonationItemRequest
                        {
                            SupplyName = i.SupplyName,
                            Quantity = i.Quantity,
                            Unit = i.Unit
                        }).ToList()!
                        : new List<RescueHub.Application.Contracts.Donation.DonationItemRequest>(),
                    src.DonationDate
                ));

            config.NewConfig<ResendOtpRequest, ResendOtpCommand>()
                .MapWith(src => new ResendOtpCommand(
                    src.Email
                ));

            config.NewConfig<RegisterRequest, RegisterCommand>()
                .MapWith(src => new RegisterCommand(
                    src.Email,
                    src.OtpCode
                ));

            config.NewConfig<LoginRequest, LoginCommand>()
                .MapWith(src => new LoginCommand(
                    src.Email,
                    src.Password
                ));

            config.NewConfig<GetUsersRequest, QueryRequest>()
                .MapWith(request => new QueryRequest
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Search = request.Search
                });

            config.NewConfig<CreateUserRequest, CreateUserCommand>()
                .MapWith(request => new CreateUserCommand(
                    request.RoleId,
                    request.Province,
                    request.FullName,
                    request.Email,
                    request.Phone,
                    request.DateOfBirth,
                    request.Gender,
                    request.Password,
                    Guid.Empty
                ));

            config.NewConfig<SubmitVolunteerProfileRequest, SubmitVolunteerProfileCommand>()
                .MapWith(request => new SubmitVolunteerProfileCommand(
                    Guid.Empty,
                    request.ExperienceYears,
                    request.CVUrl,
                    request.Skills
                        .Select(s => new VolunteerSkillInput(s.SkillId, s.Level))
                        .ToList()
                ));

            config.NewConfig<AuditLogQueryRequest, QueryRequest>();
            config.NewConfig<NotificationQueryRequest, QueryRequest>();
            config.NewConfig<VolunteerQueryRequest, QueryRequest>();

            // DTO -> Response
            config.NewConfig<UserProfileDto, UserProfileResponse>();
            config.NewConfig<UserProfileDto, GetProfileResponse>();
            config.NewConfig<UserListDto, UserListResponse>();
            config.NewConfig<PaginationResponse<UserListDto>, PaginationResponse<UserListResponse>>();
            config.NewConfig<UserDetailDto, UserDetailResponse>();
            config.NewConfig<CreateUserDto, CreateUserResponse>();
            config.NewConfig<UserStatusDto, UserStatusResponse>();
            config.NewConfig<AuditLogDto, AuditLogResponse>();
            config.NewConfig<NotificationDto, NotificationResponse>();
            config.NewConfig<DonationDto, GetMyDonationResponse>();
            config.NewConfig<VolunteerProfileDto, VolunteerProfileResponse>();
            config.NewConfig<VolunteerSkillDto, VolunteerSkillResponse>();
        }
    }
}