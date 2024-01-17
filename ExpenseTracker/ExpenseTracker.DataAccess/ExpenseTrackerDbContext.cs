using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.DataAccess
{
    public class ExpenseTrackerDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        private readonly string databaseFilePath;

        public ExpenseTrackerDbContext(string connectionString)
        {
            this.databaseFilePath = connectionString;
            Database.EnsureCreated();
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(databaseFilePath); 
        }
    }
}
