using Mapster;
using RescueHub.API.Models;
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
        }
    }

}
