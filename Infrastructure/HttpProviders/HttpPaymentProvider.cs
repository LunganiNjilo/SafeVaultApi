using Application.Interfaces;
using Application.Models;
using System.Net.Http.Json;


namespace Infrastructure.HttpProviders
{
    public class HttpPaymentProvider : IHttpPaymentProvider
    {
        private readonly HttpClient _http;

        public HttpPaymentProvider(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaymentResult?> SendPaymentAsync()
        {
            return await _http.GetFromJsonAsync<PaymentResult>("/pay");
        }
    }
}
