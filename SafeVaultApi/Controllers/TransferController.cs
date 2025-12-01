using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var requestCommad = TransferMapper.FromTransferFundsRequest(request);

            var result = await _transferService.TransferAsync(requestCommad);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
