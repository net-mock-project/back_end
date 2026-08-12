using Mapster;
using RescueHub.API.Models;
using RescueHub.Application.Features.Users.Commands;
using RescueHub.Application.Features.Auth.Commands.SendOtp;
using RescueHub.Application.Features.Auth.Commands.ResendOtp;
using RescueHub.Application.Features.Auth.Commands.Register;


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

            config.NewConfig<ResendOtpRequest, ResendOtpCommand>()
                .MapWith(src => new ResendOtpCommand(
                    src.Email
                ));

            config.NewConfig<RegisterRequest, RegisterCommand>()
                .MapWith(src => new RegisterCommand(
                    src.Email,
                    src.OtpCode
                ));
        }
    }

}
