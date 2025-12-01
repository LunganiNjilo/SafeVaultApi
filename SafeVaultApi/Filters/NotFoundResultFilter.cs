using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeVaultApi.Filters
{
    public class NotFoundResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is NotFoundResult)
            {
                context.Result = new JsonResult(new
                {
                    error = "Resource not found"
                })
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            await next();
        }
    }
}
