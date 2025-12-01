using Application.Models;
using SafeVaultApi.Models.Request;

namespace SafeVaultApi.Mapping
{
    public class PaymentMapper
    {
        public static PayCommand FromPayRequest(PayRequest request)
        {
            return new PayCommand
            {
                FromAccountId = request.FromAccountId,          
                Amount = request.Amount,
                Description = request.Description
            };
        }
    }
}
