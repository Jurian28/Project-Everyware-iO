namespace SharedClassLibrary.ApiResponse;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }

    public static ApiResponse<T> Ok(T? data) => new ApiResponse<T>
    {
        Success = true,
        Data = data,
        Error = null
    };

    public static ApiResponse<T> Fail(string error, T? data = default) => new ApiResponse<T>
    {
        Success = false,
        Data = data,
        Error = error
    };
}