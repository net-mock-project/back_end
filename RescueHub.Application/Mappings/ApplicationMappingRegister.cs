using Mapster;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Domain.Common.Querying;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Entities;

namespace RescueHub.Application.Mappings
{
    public class ApplicationMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FilterRequest, FilterCriteria>()
                .Map(
                    dest => dest.Operator,
                    src => (Domain.Common.Querying.FilterOperator)src.Operator);

            config.NewConfig<QueryRequest, QueryCriteria>()
                .Map(
                    dest => dest.SortDirection,
                    src => (Domain.Common.Querying.SortDirection)src.SortDirection);

            // Map Domain Entity sang DTO
            config.NewConfig<User, UserProfileDto>();
        }
    }
}