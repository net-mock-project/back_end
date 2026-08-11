namespace RescueHub.Domain.Common.Querying
{
    public class FilterCriteria
    {
        public string Field { get; set; } = null!;
        public string? Value { get; set; }
        public FilterOperator Operator { get; set; }
            = FilterOperator.Equals;
    }
}
