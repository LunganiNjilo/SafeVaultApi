using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/airtime")]
    public class AirtimeController : ControllerBase
    {
        private readonly IAirtimeService _airtime;

        public AirtimeController(IAirtimeService airtime)
        {
            _airtime = airtime;
        }

        [HttpPost("purchase")]
        public async Task<ActionResult<AirtimeResponse>> Purchase(AirtimeRequest request)
        {
            var result = await _airtime.PurchaseAirtimeAsync(
                request.AccountNumber,
                request.Description,
                request.Amount
            );

            return Ok(new AirtimeResponse
            {
                IsSuccess = result
            });
        }
    }

}
