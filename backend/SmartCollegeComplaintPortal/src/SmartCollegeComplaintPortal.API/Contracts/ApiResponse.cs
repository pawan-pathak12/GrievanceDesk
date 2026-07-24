namespace DotNetApiStarterKit.Api.Contracts
{
    /// <summary>
    /// Standard success response. Used when result.IsSuccess == true.
    /// </summary>
    public record ApiResponse(bool IsSuccess, int StatusCode, object? Data, List<string>? Errors = null);

    /// <summary>
    /// Standard error response. Used when result.IsSuccess == false AND in ExceptionMiddleware.
    /// </summary>
    public record ErrorResponse(bool IsSuccess, int StatusCode, object? Data, List<string> Errors);
}
