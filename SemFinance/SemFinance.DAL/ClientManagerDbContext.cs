using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SemFinance.Model;

namespace SemFinance.DAL
{
    public class ClientManagerDbContext : IdentityDbContext<AppUser>
    {
        public ClientManagerDbContext(DbContextOptions<ClientManagerDbContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Currency> Currency { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Currency>().HasData(new Currency { ID = 1, Name = "Hrvatska kuna", CurrencyCode = "HRK", KonversionValue = 1 });
            builder.Entity<Currency>().HasData(new Currency { ID = 2, Name = "Euro", CurrencyCode = "EUR", KonversionValue = 0.13 });
            builder.Entity<Currency>().HasData(new Currency { ID = 3, Name = "Američki dolar", CurrencyCode = "USD", KonversionValue = 0.14 });
        }
    }
}
