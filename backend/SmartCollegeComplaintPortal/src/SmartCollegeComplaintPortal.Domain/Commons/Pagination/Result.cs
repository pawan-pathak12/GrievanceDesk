namespace DotNetApiStarterKit.Domain.Commons.Pagination
{
    public class Result
    {
        public bool IsSuccess { get; }
        public int? Id { get; set; }
        public object? Data { get; set; }
        public List<string> Errors { get; }
        public int StatusCode { get; }
        public string? Message { get; set; }

        private Result(bool isSuccess, object? data, List<string>? errors,
            int statusCode, int? id, string? message = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Errors = errors ?? new List<string>();
            StatusCode = statusCode;
            Id = id;
            Message = message;
        }

        // ── Success Factories ──

        public static Result Success(object? data = null, int statusCode = 200)
            => new(true, data, null, statusCode, 0);

        public static Result Success()
            => new(true, null, null, 200, 0);

        public static Result Success(int id)
            => new(true, null, null, 200, id);

        public static Result Success(string message)
            => new(true, null, null, 200, 0, message);

        public static Result Success(object data, string message)
            => new(true, data, null, 200, 0, message);

        // ── Failure Factories ──

        public static Result Failure(string error, int statusCode = 400)
            => new(false, null, new List<string> { error }, statusCode, 0);

        public static Result Failure(List<string> errors, int statusCode = 400)
            => new(false, null, errors, statusCode, 0);

        public static Result NotFound(string error)
            => new(false, null, new List<string> { error }, 404, 0);

        public static Result Unauthorized(string error)
            => new(false, null, new List<string> { error }, 401, 0);

        public static Result Forbidden(string error)
            => new(false, null, new List<string> { error }, 403, 0);
    }

}