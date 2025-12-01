using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Filters;

namespace SafeVaultApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomErrorHandling(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<NotFoundResultFilter>();
            });

            // Handle 400s (bad requests / validation errors)
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Title = "Validation Failed",
                        Status = StatusCodes.Status400BadRequest
                    };

                    return new BadRequestObjectResult(problemDetails);
                };
            });

            return services;
        }
    }
}
