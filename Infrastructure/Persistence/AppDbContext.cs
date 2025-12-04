using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- User ---
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("tb_Users");
                b.HasKey(u => u.Id);
                b.Property(u => u.FirstName)
                 .IsRequired()
                 .HasMaxLength(100);

                b.Property(u => u.LastName)
                 .IsRequired()
                 .HasMaxLength(100);

                b.Property(u => u.IdNumber)
                 .IsRequired()
                 .HasMaxLength(13); // SA ID length

                b.Property(u => u.DateOfBirth)
                 .IsRequired();

                b.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                b.HasIndex(u => u.Email).IsUnique();
                b.HasIndex(u => u.IdNumber).IsUnique();
            });

            // --- Account ---
            modelBuilder.Entity<Account>(b =>
            {
                b.ToTable("tb_Accounts");
                b.HasKey(a => a.Id);

                b.Property(a => a.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(30);

                b.Property(a => a.Name)
                 .IsRequired()
                 .HasMaxLength(100);

                b.Property(a => a.AccountType)
                 .HasConversion<int>()
                  .IsRequired();

                b.HasIndex(a => a.AccountNumber).IsUnique();

                b.Property(a => a.Balance).HasPrecision(18, 2);

                b.Property(a => a.RowVersion)
                 .IsRowVersion();

                b.HasOne(a => a.User)
                    .WithMany(u => u.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Transaction ---
            modelBuilder.Entity<Transaction>(b =>
            {
                b.ToTable("tb_Transactions");
                b.HasKey(t => t.Id);

                // ⭐ All financial fields must be precise
                b.Property(t => t.Amount).HasPrecision(18, 2);
                b.Property(t => t.Fee).HasPrecision(18, 2);
                b.Property(t => t.BalanceAfter).HasPrecision(18, 2);

                b.HasOne(t => t.Account)
                    .WithMany(a => a.Transactions)
                    .HasForeignKey(t => t.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(t => t.CreatedAt);
            });
        }
    }
}
