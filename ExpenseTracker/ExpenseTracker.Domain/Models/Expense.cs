using ExpenseTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ExpenseTracker.Domain.Models
{
    public class Expense
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public CurrencyPreference CurrencyPreference;
        [NotMapped]
        public string FormattedAmount
        {
            get 
            {
                if (CurrencyPreference == CurrencyPreference.RON)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", Math.Round(Amount, 2), CurrencyPreference);
                }

                if (CurrencyPreference == CurrencyPreference.EUR)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", "€", Math.Round(Amount, 2));
                }

                if (CurrencyPreference == CurrencyPreference.USD)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", "$", Math.Round(Amount, 2));
                }

                return Amount.ToString();
            }
        }


        public Expense() { }

        public Expense(string name, decimal amount, CurrencyPreference currencyPreference)
        {
            Name = name;
            Amount = amount;
            CurrencyPreference = currencyPreference;
        }
    }
}
