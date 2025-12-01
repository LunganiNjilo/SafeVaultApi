using Application.Models;
using System.Net.Http.Json;

namespace Application.Interfaces
{
    public interface IHttpPaymentProvider
    {
        Task<PaymentResult> SendPaymentAsync();
    }
}
