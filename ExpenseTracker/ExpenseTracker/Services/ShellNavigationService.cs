using ExpenseTracker.Pages;

namespace ExpenseTracker.Services
{
    public class ShellNavigationService : IShellNavigationService
    {
        public async Task NavigateToAddExpensePageAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddExpensePage));
        }

        public async Task NavigateToMainPageAsync()
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }

        public async Task NavigateToSettingsPageAsync()
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }
    }
}
