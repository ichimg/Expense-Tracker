using ExpenseTracker.DataAccess.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.DataAccess.Repositories
{
    public class ExpensesRepository : IExpensesRepository
    {
        private readonly ExpenseTrackerDbContext dbContext;

        public ExpensesRepository(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Create(Expense expense)
        {
            await dbContext.Expenses.AddAsync(expense);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Expense>> GetAll()
        {
            return await dbContext.Expenses.OrderByDescending(e => e.Date)
                                           .AsNoTracking()
                                           .ToListAsync();
        }

        public async Task<Expense> GetById(int id)
        {
            return await dbContext.Expenses.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task Delete(int id)
        {
            var expenseFromDb = await GetById(id);

            if (expenseFromDb == null)
            {
                throw new EntityNotFoundException(id);
            }

            dbContext.Expenses.Remove(expenseFromDb);

            await dbContext.SaveChangesAsync();
        }
    }
}
