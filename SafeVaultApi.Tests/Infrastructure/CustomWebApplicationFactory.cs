using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SafeVaultApi.Tests.Infrastructure
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureTestServices;

        public CustomWebApplicationFactory(Action<IServiceCollection> configureTestServices)
        {
            _configureTestServices = configureTestServices;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // ⭐ Ensure Program.cs knows we are in test mode
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Apply repository mocks and overrides
                _configureTestServices?.Invoke(services);
            });
        }
    }
}
