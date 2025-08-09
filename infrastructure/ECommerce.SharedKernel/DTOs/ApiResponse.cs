namespace ECommerce.SharedKernel.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Code { get; set; } = "OK";
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> Ok(T? data, string message = "") => new()
        {
            Success = true,
            Code = "OK",
            Message = message,
            Data = data,
            TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString()
        };

        public static ApiResponse<T> Fail(string code, string message, T? data = default) => new()
        {
            Success = false,
            Code = code,
            Message = message,
            Data = data,
            TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString()
        };
    }
}
