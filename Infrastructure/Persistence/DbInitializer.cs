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

            // ============================
            // PRIMARY USER (Demo User)
            // ============================
            var user1 = new User
            {
                FirstName = "Lungani",
                LastName = "Nhilo",
                IdNumber = "9001015800081",
                DateOfBirth = new DateTime(1990, 01, 01),
                Email = "test@safesystems.dev",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123")
            };

            await db.Users.AddAsync(user1);

            var user1Current = new Account
            {
                User = user1,
                Name = "Current Account",
                AccountNumber = "SV-1000001",
                Balance = 1000M,
                Currency = "ZAR",
                AccountType = AccountType.Current
            };

            var user1Savings = new Account
            {
                User = user1,
                Name = "Savings Account",
                AccountNumber = "SV-1000002",
                Balance = 500M,
                Currency = "ZAR",
                AccountType = AccountType.Savings
            };

            await db.Accounts.AddRangeAsync(user1Current, user1Savings);

            await db.Transactions.AddAsync(new Transaction
            {
                Account = user1Current,
                Amount = 1000M,
                Fee = 0,
                Type = TransactionType.Credit,
                Description = "Initial balance",
                BalanceAfter = 1000M
            });

            await db.Transactions.AddAsync(new Transaction
            {
                Account = user1Savings,
                Amount = 1000M,
                Fee = 0,
                Type = TransactionType.Credit,
                Description = "Initial balance",
                BalanceAfter = 1000M
            });

            // ============================
            // SECONDARY USER
            // ============================
            var user2 = new User
            {
                FirstName = "Alex",
                LastName = "Molefe",
                IdNumber = "9205124800089",
                DateOfBirth = new DateTime(1992, 05, 12),
                Email = "user@safevault.io",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123")
            };

            await db.Users.AddAsync(user2);

            var user2Current = new Account
            {
                User = user2,
                Name = "Everyday Account",
                AccountNumber = "SV-2000001",
                Balance = 750M,
                Currency = "ZAR",
                AccountType = AccountType.Current
            };

            var user2Savings = new Account
            {
                User = user2,
                Name = "Savings Account",
                AccountNumber = "SV-2000002",
                Balance = 300M,
                Currency = "ZAR",
                AccountType = AccountType.Savings
            };

            await db.Accounts.AddRangeAsync(user2Current, user2Savings);

            await db.Transactions.AddAsync(new Transaction
            {
                Account = user2Current,
                Amount = 750M,
                Fee = 0,
                Type = TransactionType.Credit,
                Description = "Initial balance",
                BalanceAfter = 750M
            });

            await db.Transactions.AddAsync(new Transaction
            {
                Account = user2Savings,
                Amount = 750M,
                Fee = 0,
                Type = TransactionType.Credit,
                Description = "Initial balance",
                BalanceAfter = 750M
            });

            await db.SaveChangesAsync();
        }
    }
}
