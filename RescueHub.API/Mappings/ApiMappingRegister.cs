using Mapster;
using RescueHub.API.Models.AuditLogs;
using RescueHub.API.Models.Auth;
using RescueHub.API.Models.Users;
using RescueHub.Application.Contracts.AuditLogs;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Application.Features.Auth.Commands;
using RescueHub.Application.Features.Users.Commands;

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
                .MapWith(request => new SendOtpCommand(
                    request.FullName,
                    request.DateOfBirth,
                    request.Email,
                    request.Phone,
                    request.Gender,
                    request.Password,
                    request.Address
                ));

            config.NewConfig<ResendOtpRequest, ResendOtpCommand>()
                .MapWith(request => new ResendOtpCommand(
                    request.Email
                ));

            config.NewConfig<RegisterRequest, RegisterCommand>()
                .MapWith(request => new RegisterCommand(
                    request.Email,
                    request.OtpCode
                ));


            config.NewConfig<LoginRequest, LoginCommand>()
                .MapWith(request => new LoginCommand(
                    request.Email,
                    request.Password
                ));

            config.NewConfig<AuditLogQueryRequest, QueryRequest>();

            // DTO -> Response
            config.NewConfig<UserProfileDto, UserProfileResponse>();
            config.NewConfig<UserProfileDto, GetProfileResponse>();
            config.NewConfig<AuditLogDto, AuditLogResponse>();
        }
    }

}
