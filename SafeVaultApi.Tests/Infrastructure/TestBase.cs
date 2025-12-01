using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace SafeVaultApi.Tests.Infrastructure
{
    public abstract class TestBase : IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient HttpClient { get; }

        // Expose the mock so tests can configure it in Arrange
        protected IAccountRepository accountRepositoryMock { get; }
        protected ITransactionRepository transactionRepositoryMock { get; }
        protected IUnitOfWork uowMock { get; }

        protected TestBase()
        {
            // create substitute once (tests can re-configure per-test)
            accountRepositoryMock = Substitute.For<IAccountRepository>();
            transactionRepositoryMock = Substitute.For<ITransactionRepository>();
            uowMock = Substitute.For<IUnitOfWork>();

            // create factory and register substitutes in DI
            _factory = new CustomWebApplicationFactory<Program>(services =>
            {
                // replace the real registrations with test doubles
                services.AddScoped(_ => accountRepositoryMock);
                services.AddScoped(_ => transactionRepositoryMock);
                services.AddScoped(_ => uowMock);
                // register any other mocks here if you want defaults
            });

            HttpClient = _factory.CreateClient();
        }

        /// <summary>
        /// Create a strongly-typed API client wrapper using the shared HttpClient.
        /// Call this from each test's [SetUp].
        /// </summary>
        protected TClient CreateApiClient<TClient>() where TClient : class
        {
            return (TClient)Activator.CreateInstance(typeof(TClient), HttpClient)!;
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
            _factory?.Dispose();
        }
    }

}
