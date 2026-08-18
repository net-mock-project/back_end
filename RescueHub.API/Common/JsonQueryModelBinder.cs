using Microsoft.AspNetCore.Mvc.ModelBinding;
using RescueHub.Application.Contracts.Querying;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace RescueHub.API.Common
{
    public class JsonQueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // Tên tham số query (VD: "Filters" hoặc "filters")
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                // Thử tìm theo biến viết thường nếu không tìm thấy exact name
                valueProviderResult = bindingContext.ValueProvider.GetValue("filters");
                if (valueProviderResult == ValueProviderResult.None)
                    return Task.CompletedTask;
            }

            var rawValue = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(rawValue))
                return Task.CompletedTask;

            try
            {
                var decodedValue = HttpUtility.UrlDecode(rawValue).Trim();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());

                // Trường hợp 1: Dạng Mảng JSON [...]
                if (decodedValue.StartsWith("["))
                {
                    var result = JsonSerializer.Deserialize<List<FilterRequest>>(decodedValue, options);
                    bindingContext.Result = ModelBindingResult.Success(result ?? new List<FilterRequest>());
                }
                // Trường hợp 2: Dạng Object JSON đơn {...}
                else if (decodedValue.StartsWith("{"))
                {
                    var singleItem = JsonSerializer.Deserialize<FilterRequest>(decodedValue, options);
                    if (singleItem != null)
                    {
                        bindingContext.Result = ModelBindingResult.Success(new List<FilterRequest> { singleItem });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Binder Error]: {ex.Message}");
                bindingContext.ModelState.TryAddModelError(modelName, $"Không thể deserialize JSON Filter: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
