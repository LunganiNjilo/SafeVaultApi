using Application.Models;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResult> PayAsync(PayCommand request);
    }
}
