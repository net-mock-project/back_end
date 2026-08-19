using System.Net;
using FluentValidation;
using Microsoft.Data.SqlClient;
using RescueHub.API.Models;
using RescueHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace RescueHub.API.Common
{
    /// <summary>
    /// Bắt MỌI exception chưa được xử lý và trả về đúng envelope ApiResponse.
    /// Nhờ đó client luôn nhận cùng một hình dạng kể cả khi có lỗi.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, messages) = exception switch
            {
                ValidationException validationException =>
                (
                    HttpStatusCode.BadRequest,
                    validationException.Errors
                        .Select(error => error.ErrorMessage)
                        .ToArray()
                ),

                NotFoundException =>
                    (HttpStatusCode.NotFound,
                    new[] { exception.Message }),

                ConflictException =>
                    (HttpStatusCode.Conflict,
                    new[] { exception.Message }),

                // 400
                ArgumentException =>
                    (HttpStatusCode.BadRequest,
                    new[] { exception.Message }),

                InvalidOperationException =>
                    (HttpStatusCode.BadRequest,
                    new[] { exception.Message }),

                // 401
                UnauthorizedAccessException =>
                    (HttpStatusCode.Unauthorized,
                    new[] { exception.Message }),

                // 409
                SqlException sqlException
                    when sqlException.Number == 2601 ||
                         sqlException.Number == 2627
                    =>
                    (HttpStatusCode.Conflict,
                    new[]
                    {
                        "Email hoặc số điện thoại đã được sử dụng."
                    }),

                // 500
                _ =>
                    (HttpStatusCode.InternalServerError,
                    new[]
                    {
                        "An unexpected error occurred."
                    })
            };

            // Chỉ log chi tiết cho lỗi ngoài dự kiến; không rò rỉ thông tin ra client.
            if (statusCode ==
                HttpStatusCode.InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception while processing {Path}",
                    httpContext.Request.Path);
            }

            var response =
                ApiResponse.Fail(
                    statusCode,
                    messages);

            httpContext.Response.StatusCode =
                (int)statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            return true;
        }
    }
}