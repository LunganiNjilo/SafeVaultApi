using Application.Enums;

namespace Application.Common.Exceptions
{
    public static class ErrorCatalog
    {
        private static readonly Dictionary<ErrorType, (string Code, string Message)> _errors =
            new()
            {
            { ErrorType.NotFound, ("AV404", "The requested resource was not found.") },
            { ErrorType.BadRequest, ("AV400", "The request was invalid.") },
            { ErrorType.InternalServerError, ("AV500", "An unexpected error occurred.") },
            { ErrorType.Unauthorized, ("AV401", "Unauthorized User.") }
            };

        public static (string Code, string Message) Get(ErrorType errorType)
        {
            return _errors[errorType];
        }
    }
}
