using Application.Common.Exceptions;
using Application.Enums;
using Domain.Errors;

namespace SafeVaultApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var errorResponse = new ApiError
                {
                    StatusCode = ex.StatusCode,
                    ErrorCode = ex.ErrorCode,
                    Message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var error = ErrorCatalog.Get(ErrorType.InternalServerError);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new ApiError
                {
                    StatusCode = 500,
                    ErrorCode = error.Code,
                    Message = error.Message
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
