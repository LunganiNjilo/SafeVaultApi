using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (await db.Users.AnyAsync())
                return;

            var user = new User
            {
                FirstName = "Lungani",
                LastName = "Nhilo",
                IdNumber = "9001015800081",
                DateOfBirth = new DateTime(1990, 01, 01),
                Email = "test@local.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
            };

            await db.Users.AddAsync(user);

            // --- SOURCE ACCOUNT (Current) ---
            var currentAccount = new Account
            {
                User = user,
                Name = "Current Account",
                AccountNumber = "SV-1000001",
                Balance = 1000M,
                Currency = "ZAR",
                AccountType = AccountType.Current
            };

            // --- DESTINATION ACCOUNT (Savings) ---
            var savingsAccount = new Account
            {
                User = user,
                Name = "My Savings Account",
                AccountNumber = "SV-1000002",
                Balance = 0M,
                Currency = "ZAR",
                AccountType = AccountType.Savings
            };

            await db.Accounts.AddRangeAsync(currentAccount, savingsAccount);

            // ---- INITIAL BALANCE TRANSACTION ----
            var initialTx = new Transaction
            {
                Account = currentAccount,
                Amount = 1000M,
                Fee = 0,
                Type = TransactionType.Credit,
                Description = "Initial balance",
                BalanceAfter = 1000M
            };

            await db.Transactions.AddAsync(initialTx);

            // ===============================
            // INTERNAL TRANSFER: R200
            // FROM Current > Savings
            // ===============================

            decimal transferAmount = 200M;

            // Debit from Current
            var debitTx = new Transaction
            {
                Account = currentAccount,
                Amount = transferAmount,
                Fee = 0,
                Type = TransactionType.Debit,
                Description = "Transfer to Savings",
                BalanceAfter = currentAccount.Balance - transferAmount
            };

            currentAccount.Balance -= transferAmount;

            // Credit to Savings
            var creditTx = new Transaction
            {
                Account = savingsAccount,
                Amount = transferAmount,
                Fee = 0,
                Type = TransactionType.Credit,
                Description = "Transfer from Current",
                BalanceAfter = savingsAccount.Balance + transferAmount
            };

            savingsAccount.Balance += transferAmount;

            await db.Transactions.AddRangeAsync(debitTx, creditTx);

            await db.SaveChangesAsync();
        }
    }
}
