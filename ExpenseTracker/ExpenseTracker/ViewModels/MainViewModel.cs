using ExpenseTracker.DataAccess.Exceptions;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ExpenseTracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IExpensesRepository expensesRepository;
        private readonly IShellNavigationService shellNavigationService;
        private readonly ICurrenciesRepository currenciesRepository;
        public MainViewModel(IExpensesRepository expensesRepository, IShellNavigationService shellNavigationService, ICurrenciesRepository currenciesRepository)
        {
            this.expensesRepository = expensesRepository;
            this.shellNavigationService = shellNavigationService;
            this.currenciesRepository = currenciesRepository;

            Init();
        }

        public delegate void OnConvertExpensesDelegate(CurrencyPreference preference);
        public event OnConvertExpensesDelegate OnConvertExpenses;

        private async Task Init()
        {
            var expensesFromDb = await expensesRepository.GetAll();
            Expenses = new ObservableCollection<Expense>(expensesFromDb);

            if (Expenses.Count == 0)
            {

                IsEmptyList = true;
                return;
            }

            CurrencyPreference preference =
            (CurrencyPreference)Enum.Parse(typeof(CurrencyPreference),
            Preferences.Get("CurrencyPreference", CurrencyPreference.RON.ToString()));

            foreach (var expense in Expenses)
            {
                expense.CurrencyPreference = preference;
            }

            await ConvertExpenses(CurrencyPreference.RON.ToString(), preference);
        }

        private bool isEmptyList;

        public bool IsEmptyList
        {
            get { return isEmptyList; }
            set
            {
                if (isEmptyList != value)
                {
                    isEmptyList = value;
                    NotifyPropertyChanged(nameof(IsEmptyList));
                }
            }
        }


        private ObservableCollection<Expense> expenses;
        public ObservableCollection<Expense> Expenses
        {
            get
            {
                return expenses;
            }

            set
            {
                expenses = value;
                NotifyPropertyChanged(nameof(Expenses));
            }
        }

        #region OpenAddExpensePage
        public ICommand OpenAddExpensePageCommand => new Command(async ()
            => await shellNavigationService.NavigateToAddExpensePageAsync());
        #endregion

        #region DeleteExpense
        public ICommand DeleteExpenseCommand => new Command<Expense>(async (expense) =>
        {
            try
            {
                await expensesRepository.Delete(expense.Id);
                Expenses.Remove(expense);
                Expenses = new ObservableCollection<Expense>(Expenses);

                if (Expenses.Count == 0)
                {
                    IsEmptyList = true;
                }
            }
            catch (EntityNotFoundException ex)
            {
                Trace.TraceError(ex.Message);
            }

        });
        #endregion

        #region OpenSettingsPage
        public ICommand OpenSettingsCommand => new Command(async ()
            => await shellNavigationService.NavigateToSettingsPageAsync());
        #endregion

        public async Task ConvertExpenses(string previousCurrencyPreference, CurrencyPreference newCurrencyPreference)
        {
            if (Expenses.Count == 0)
            {
                return;
            }

            OnConvertExpenses?.Invoke(newCurrencyPreference);

            switch (newCurrencyPreference)
            {
                case CurrencyPreference.RON:
                    if (previousCurrencyPreference == CurrencyPreference.RON.ToString())
                    {
                        return;
                    }
                    await Init();
                    break;

                case CurrencyPreference.EUR:
                    if (previousCurrencyPreference == CurrencyPreference.EUR.ToString())
                    {
                        return;
                    }

                    if (previousCurrencyPreference == CurrencyPreference.RON.ToString())
                    {
                        await ExchangeRonToEurAsync(newCurrencyPreference);
                    }

                    if (previousCurrencyPreference == CurrencyPreference.USD.ToString())
                    {
                        await ExchangeUsdToEurAsync(newCurrencyPreference);
                    }

                    Expenses = new ObservableCollection<Expense>(Expenses);
                    break;

                case CurrencyPreference.USD:
                    if (previousCurrencyPreference == CurrencyPreference.USD.ToString())
                    {
                        return;
                    }

                    if (previousCurrencyPreference == CurrencyPreference.RON.ToString())
                    {
                        await ExchangeRonToUsdAsync(newCurrencyPreference);
                    }

                    if (previousCurrencyPreference == CurrencyPreference.EUR.ToString())
                    {
                        await ExchangeEurToUsdAsync(newCurrencyPreference);
                    }


                    Expenses = new ObservableCollection<Expense>(Expenses);
                    break;
            }
        }

        #region Exchange Methods
        private async Task ExchangeRonToEurAsync(CurrencyPreference preference)
        {
            foreach (var expense in Expenses)
            {
                Currency currency = await currenciesRepository.GetExchangeRatesForExpense(expense);
                expense.Amount *= currency.EurExchangeRate;
                expense.CurrencyPreference = preference;
            }
        }

        private async Task ExchangeRonToUsdAsync(CurrencyPreference preference)
        {
            foreach (var expense in Expenses)
            {
                Currency currency = await currenciesRepository.GetExchangeRatesForExpense(expense);
                expense.Amount *= currency.UsdExchangeRate;
                expense.CurrencyPreference = preference;
            }
        }

        private async Task ExchangeEurToUsdAsync(CurrencyPreference preference)
        {
            foreach (var expense in Expenses)
            {
                Currency currency = await currenciesRepository.GetExchangeRatesForExpense(expense);
                expense.Amount /= currency.EurExchangeRate;
                expense.Amount *= currency.UsdExchangeRate;
                expense.CurrencyPreference = preference;
            }
        }

        private async Task ExchangeUsdToEurAsync(CurrencyPreference preference)
        {
            foreach (var expense in Expenses)
            {
                Currency currency = await currenciesRepository.GetExchangeRatesForExpense(expense);
                expense.Amount /= currency.UsdExchangeRate;
                expense.Amount *= currency.EurExchangeRate;
                expense.CurrencyPreference = preference;
            }
        }
        #endregion
    }
}
