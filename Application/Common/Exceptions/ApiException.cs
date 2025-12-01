using Application.Enums;

namespace Application.Common.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        public ApiException(int statusCode, ErrorType errorType, string? customMessage = null)
            : base(customMessage ?? ErrorCatalog.Get(errorType).Message)
        {
            StatusCode = statusCode;
            ErrorCode = ErrorCatalog.Get(errorType).Code;
        }
    }
}
