using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

  
    }
}