using RescueHub.Domain.Common.Querying;

namespace RescueHub.Application.Contracts.Querying
{
    public class FilterRequest
    {
        public string Field { get; set; } = null!;

        public string? Value { get; set; }

        public FilterOperator Operator { get; set; }
            = FilterOperator.Equals;
    }
}
