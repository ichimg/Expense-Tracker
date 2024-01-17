using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Pages;

public partial class AddExpensePage : ContentPage
{
	public AddExpensePage(AddExpenseViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}