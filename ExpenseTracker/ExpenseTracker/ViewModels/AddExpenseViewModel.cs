using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ExpenseTracker.ViewModels
{
    public class AddExpenseViewModel : ViewModelBase, IDisposable
    {
        private readonly IExpensesRepository expensesRepository;
        private readonly MainViewModel mainViewModel;
        private readonly ICurrenciesRepository currenciesRepository;

        public AddExpenseViewModel(IExpensesRepository expensesRepository, MainViewModel mainViewModel, ICurrenciesRepository currenciesRepository)
        {
            this.expensesRepository = expensesRepository;
            this.mainViewModel = mainViewModel;
            this.currenciesRepository = currenciesRepository;
            Preference = Preferences.Get("CurrencyPreference", "RON");

            mainViewModel.OnConvertExpenses += OnConvertExpenses;
        }

        private void OnConvertExpenses(CurrencyPreference preference)
        {
            Preference = preference.ToString();
        }

        private string preference;
        public string Preference
        {
            get
            {
                return preference;
            }
            set
            {
                preference = value;
                NotifyPropertyChanged(nameof(Preference));
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }
        private decimal amount;
        public decimal Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }

        public ICommand CreateExpenseCommand => new Command(async ()  =>

        {
            var preference = (CurrencyPreference)Enum.Parse(typeof(CurrencyPreference),
                Preferences.Get("CurrencyPreference", CurrencyPreference.RON.ToString()));

            var createdExpense = new Expense(Name, Amount, preference);

            if (preference != CurrencyPreference.RON)
            { 
                Currency currency = await currenciesRepository.GetExchangeRatesForExpense(createdExpense);

                if (preference == CurrencyPreference.EUR)
                {
                    createdExpense.Amount /= currency.EurExchangeRate;

                    await expensesRepository.Create(createdExpense);

                    createdExpense.Amount *= currency.EurExchangeRate;
                    mainViewModel.Expenses.Add(createdExpense);
                }

                if (preference == CurrencyPreference.USD)
                {
                    createdExpense.Amount /= currency.UsdExchangeRate;

                    await expensesRepository.Create(createdExpense);

                    createdExpense.Amount *= currency.UsdExchangeRate;
                    mainViewModel.Expenses.Add(createdExpense);
                }
            }
            else
            {
                await expensesRepository.Create(createdExpense);
                mainViewModel.Expenses.Add(createdExpense);
            }

            mainViewModel.IsEmptyList = false;
            mainViewModel.Expenses = new ObservableCollection<Expense>(mainViewModel.Expenses
                                                                        .OrderByDescending(e => e.Date));

            Name = string.Empty;
            Amount = 0;

            await Shell.Current.GoToAsync("..");
        });

        public void Dispose()
        {
            mainViewModel.OnConvertExpenses -= OnConvertExpenses;
        }
    }
}
