namespace ExpenseTracker.Services
{
    public interface IShellNavigationService
    {
        Task NavigateToAddExpensePageAsync();
        Task NavigateToMainPageAsync();
        Task NavigateToSettingsPageAsync();
    }
}