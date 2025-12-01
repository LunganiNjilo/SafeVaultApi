using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PayRequest request)
        {
            var command = PaymentMapper.FromPayRequest(request);
            var result = await _paymentService.PayAsync(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
