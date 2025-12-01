using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get(Guid accountId, int skip = 0, int take = 50)
        {
            var items = await _transactionService.GetByAccountAsync(accountId, skip, take);
            return Ok(TransactionMapper.ToResponse(items));
        }
    }
}
