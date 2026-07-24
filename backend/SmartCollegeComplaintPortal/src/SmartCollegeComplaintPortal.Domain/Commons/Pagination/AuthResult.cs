namespace DotNetApiStarterKit.Domain.Commons.Pagination
{
    public class AuthResult
    {
        public bool IsSuccess { get; }
        public bool? RequiresApproval { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; }
        public string? Error { get; }
        public int StatusCode { get; }
        public DateTime ExpiredAt { get; set; }
        public string? Message { get; set; }
        public bool IsTemporaryPassword { get; set; }

        private AuthResult(bool isSuccess, string? accessToken,
            string? refreshToken, string? error, int statusCode, string? message, bool? requiresApproval)
        {
            IsSuccess = isSuccess;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Error = error;
            StatusCode = statusCode;
            Message = message;
            RequiresApproval = requiresApproval;
        }

        public static AuthResult Success(string accessToken, string refreshToken)
              => new(true, accessToken, refreshToken, null, 200, null, null);
        public static AuthResult Success()
            => new(true, null, null, null, 200, null, null);
        public static AuthResult Success(string message)
          => new(true, null, null, null, 200, message, null);

        public static AuthResult Failure(string error, int statusCode = 400)
            => new(false, null, null, error, statusCode, null, null);
        public static AuthResult RequiredApproval(string message)
              => new(true, null, null, null, 200, message, true);
        public static AuthResult Unauthorized(string error)
            => new(false, null, null, error, 401, null, null);

    }
}
