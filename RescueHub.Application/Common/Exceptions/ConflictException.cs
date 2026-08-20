namespace RescueHub.Application.Common.Exceptions
{
    /// <summary>
    /// Ném ra khi có xung đột dữ liệu hoặc trạng thái nghiệp vụ. Được GlobalExceptionHandler ánh xạ sang HTTP 409.
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}