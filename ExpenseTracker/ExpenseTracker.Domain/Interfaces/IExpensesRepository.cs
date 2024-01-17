
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpensesRepository
    {
        Task Create(Expense expense);
        Task<List<Expense>> GetAll();
        Task<Expense> GetById(int id);
        Task Delete(int id);
    }
}