using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RescueHub.API.Models;
using RescueHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RescueHub.API.Common
{
    /// <summary>
    /// Bắt mọi exception chưa được xử lý và trả về đúng envelope ApiResponse.
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
                // Exception cũ của Application
                NotFoundException =>
                    (
                        HttpStatusCode.NotFound,
                        new[] { exception.Message }
                    ),

                // Domain Service dùng exception này khi không tìm thấy User
                KeyNotFoundException =>
                    (
                        HttpStatusCode.NotFound,
                        new[] { exception.Message }
                    ),

                // Dữ liệu đầu vào không hợp lệ
                ArgumentException =>
                    (
                        HttpStatusCode.BadRequest,
                        new[] { exception.Message }
                    ),

                // Các lỗi ngoài dự kiến
                _ =>
                    (
                        HttpStatusCode.InternalServerError,
                        new[] { "An unexpected error occurred." }
                    )
            };

            // Chỉ log chi tiết lỗi 500
            if (statusCode == HttpStatusCode.InternalServerError)
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