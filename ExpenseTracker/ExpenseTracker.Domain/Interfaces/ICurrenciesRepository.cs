
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface ICurrenciesRepository
    {
        Task<Currency> GetExchangeRatesForExpense(Expense expense);
        Task InsertCurrencyRatesAsync(Currency currency);
        Task<bool> IsCurrencyUpToDate();
    }
}