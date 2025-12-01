using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accounts;

        public AccountController(IAccountService accounts)
        {
            _accounts = accounts;
        }

        [HttpGet("{accountNumber}/balance")]
        public async Task<ActionResult<decimal>> GetBalance(string accountNumber)
        {
            return Ok(await _accounts.GetBalanceAsync(accountNumber));
        }

        [HttpPost("{accountNumber}/credit")]
        public async Task<ActionResult<TransactionResponse>> Credit(
            string accountNumber,
            [FromBody] AmountRequest request)
        {
            var tx = await _accounts.CreditAsync(
                accountNumber,
                request.Amount,
                "Deposit"
            );

            return Ok(TransactionMapper.ToResponse(tx));
        }

        [HttpPost("{accountNumber}/debit")]
        public async Task<ActionResult<TransactionResponse>> Debit(
            string accountNumber,
            [FromBody] AmountRequest request)
        {
            var tx = await _accounts.DebitAsync(
                accountNumber,
                request.Amount,
                "Debit Transaction"
            );

            return Ok(TransactionMapper.ToResponse(tx));
        }

        [HttpGet("userId/{userId}")]
        public async Task<IActionResult> GetAccountByUserIdAsync(Guid userId)
        {
            var accounts = await _accounts.GetByUserIdAsync(userId);
            return Ok(AccountMapper.ToResponse(accounts));
        }
    }
}

