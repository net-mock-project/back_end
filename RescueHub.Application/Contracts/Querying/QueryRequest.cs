using RescueHub.Domain.Common.Querying;

namespace RescueHub.Application.Contracts.Querying
{
    public class QueryRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public List<FilterRequest> Filters { get; set; } = [];

        public string? SortBy { get; set; }

        public SortDirection SortDirection { get; set; }
            = SortDirection.Asc;
    }
}
