using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Common;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Domain.Common.Querying;

namespace RescueHub.API.Models.Notifications
{
    public class NotificationQueryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }

        [ModelBinder(BinderType = typeof(JsonQueryModelBinder))]
        public List<FilterRequest> Filters { get; set; } = [];

        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
