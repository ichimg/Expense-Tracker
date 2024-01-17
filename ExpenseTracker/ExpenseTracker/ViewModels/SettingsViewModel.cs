using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace ExpenseTracker.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ICurrenciesRepository currenciesRepository;
        private readonly MainViewModel mainViewModel;

        public decimal EurExchangeRate { get; set; }
        public decimal UsdExchangeRate { get; set; }
        public SettingsViewModel(ICurrenciesRepository currenciesRepository, MainViewModel mainViewModel)
        {
            this.currenciesRepository = currenciesRepository;
            this.mainViewModel = mainViewModel;
        }

        private CurrencyPreference selectedCurrency = 
            (CurrencyPreference)Enum.Parse(typeof(CurrencyPreference), Preferences.Get("CurrencyPreference", CurrencyPreference.RON.ToString()));
        public CurrencyPreference SelectedCurrency
        {
            get

            { return selectedCurrency; }

            set
            {
                if (selectedCurrency != value)
                {
                    string previousCurrencyPreference = SelectedCurrency.ToString();
                    selectedCurrency = value;

                    NotifyPropertyChanged(nameof(SelectedCurrency));
                    Preferences.Set("CurrencyPreference", SelectedCurrency.ToString());

                    OnCurrencyChangeAsync(previousCurrencyPreference);
                }
            }
        }

        private async Task OnCurrencyChangeAsync(string previousCurrencyPreference)
        {
            bool isCurrencyUpToDate = await currenciesRepository.IsCurrencyUpToDate();
            if (!isCurrencyUpToDate)
            {
                await GetLatestCurrencyRates();
            }


            await mainViewModel.ConvertExpenses(previousCurrencyPreference, SelectedCurrency);
        }

        private async Task GetLatestCurrencyRates()
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://api.currencyapi.com/v3/latest") })
            {
                var result = await httpClient
                .GetStringAsync("?apikey=cur_live_5vkTqif3fWP5fMOKgPV5TVLy5CV3Y26odf80MfAU&currencies=EUR%2CUSD&base_currency=RON");
                var json = JObject.Parse(result);

                var data = json["data"];
                EurExchangeRate = (decimal)data["EUR"]["value"];
                UsdExchangeRate = (decimal)data["USD"]["value"];

                Currency fetchedCurrency = new Currency
                {
                    EurExchangeRate = Math.Round(EurExchangeRate, 3),
                    UsdExchangeRate = Math.Round(UsdExchangeRate, 3),
                    RequestCallDate = DateTime.Now.Date,
                };

                Trace.TraceInformation($"EUR: {EurExchangeRate}");
                Trace.TraceInformation($"USD: {UsdExchangeRate}");

                await currenciesRepository.InsertCurrencyRatesAsync(fetchedCurrency);
            }
        }


        public CurrencyPreference[] CurrencyValues => (CurrencyPreference[])Enum.GetValues(typeof(CurrencyPreference));

    }
}
