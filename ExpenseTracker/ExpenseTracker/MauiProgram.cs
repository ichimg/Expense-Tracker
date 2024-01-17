using ExpenseTracker.DataAccess;
using ExpenseTracker.DataAccess.Repositories;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Pages;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.Extensions.Logging;


namespace ExpenseTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            string databaseFilePath = $"Filename={DatabasePath.GetPath()}";

            // repositories
            builder.Services.AddSingleton(provider => new ExpenseTrackerDbContext(databaseFilePath));
            builder.Services.AddSingleton<IExpensesRepository, ExpensesRepository>();
            builder.Services.AddSingleton<ICurrenciesRepository, CurrenciesRepository>();

            // pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AddExpensePage>();
            builder.Services.AddSingleton<SettingsPage>();

            // view models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<AddExpenseViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();

            // services
            builder.Services.AddSingleton<IShellNavigationService, ShellNavigationService>();

            return builder.Build();
        }
    }
}