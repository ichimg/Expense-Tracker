using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.DataAccess.Repositories
{
    public class CurrenciesRepository : ICurrenciesRepository
    {
        private readonly ExpenseTrackerDbContext dbContext;

        public CurrenciesRepository(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task InsertCurrencyRatesAsync(Currency currency)
        {
            await dbContext.Currencies.AddAsync(currency);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Currency> GetExchangeRatesForExpense(Expense expense)
        {
            var closestCurrency =  dbContext.Currencies
                        .AsEnumerable()
                        .OrderBy(c => Math.Abs((c.RequestCallDate - expense.Date).TotalDays))
                        .FirstOrDefault();

            if (closestCurrency is null)
            {
                throw new InvalidOperationException("No suitable exchange rate found.");
            }

            return closestCurrency;
        }

        public async Task<bool> IsCurrencyUpToDate()
        {
            var currencyFromDb = await dbContext.Currencies
                                        .FirstOrDefaultAsync(c => c.RequestCallDate.Date == DateTime.Now.Date);
            bool isUpToDate =  currencyFromDb != null;

            return isUpToDate;
        }
    }
}
