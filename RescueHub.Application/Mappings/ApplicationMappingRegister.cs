using Mapster;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Domain.Common.Querying;

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
            // Quan hệ: Entity --> DTO.

            // Add custom mapping rules here as the application grows.
        }
    }
}
