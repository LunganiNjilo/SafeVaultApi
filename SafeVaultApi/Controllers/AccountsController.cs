using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;
using System.Net;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accounts;
        private readonly IManualTransactionService _manualTransactionService;

        public AccountController(IAccountService accounts, IManualTransactionService manualTransactionService)
        {
            _accounts = accounts;
            _manualTransactionService = manualTransactionService;
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

        [HttpPost("{accountId}/close")]
        public async Task<IActionResult> CloseAccount(Guid accountId)
        {
            await _accounts.CloseAccountAsync(accountId);
            return NoContent();
        }

        [HttpPost("{accountId:guid}/manual-transactions")]
        public async Task<IActionResult> CreateManualTransaction(
            Guid accountId,
            [FromBody] CreateManualTransactionRequest request)
        {
            request.AccountId = accountId;

            var result = await _manualTransactionService.CreateManualAsync(
                ManualTransactionMapper.ToCommand(request)
            );

            if (!result.Success)
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    result.Message
                );

            return Ok(new { success = true, message = result.Message });
        }

        [HttpPut("manual-transactions/{transactionId:guid}")]
        public async Task<IActionResult> UpdateManualTransaction(
            Guid transactionId,
            [FromBody] UpdateManualTransactionRequest request)
        {
            request.TransactionId = transactionId;

            var result = await _manualTransactionService.UpdateManualAsync(
                ManualTransactionMapper.ToCommand(request)
            );

            if (!result.Success)
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    result.Message
                );

            return Ok(new { success = true, message = result.Message });
        }

        [HttpDelete("manual-transactions/{transactionId:guid}")]
        public async Task<IActionResult> DeleteManualTransaction(Guid transactionId)
        {
            var result = await _manualTransactionService.DeleteManualAsync(transactionId);

            if (!result.Success)
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    result.Message
                );

            return Ok(new { success = true, message = result.Message });

        }
    }
}

