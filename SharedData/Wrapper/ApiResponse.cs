
namespace SharedData.Wrapper;

/// <summary>Standard API response envelope for all endpoints.</summary>
/// <typeparam name="T">Payload type returned in <see cref="Data"/>.</typeparam>
public class ApiResponse<T>
{
    /// <summary>True when the operation completed successfully.</summary>
    public bool Success { get; set; }

    /// <summary>Human-readable status or error message.</summary>
    public string? Message { get; set; }

    /// <summary>Response payload when <see cref="Success"/> is true.</summary>
    public T? Data { get; set; }

    /// <summary>Validation or field-level errors when <see cref="Success"/> is false.</summary>
    public object? Errors { get; set; }

	public static ApiResponse<T> SuccessResponse(T data, string message = "")
	{
		return new ApiResponse<T> { Success = true, Message = message, Data = data };
	}

	public static ApiResponse<T> FailResponse(string message, object? errors=null)
	{
		return new ApiResponse<T> { Success = false, Message = message, Errors = errors };
	}
}
