using System_losowania_osoby_do_odpowiedzi.ViewModels;

namespace System_losowania_osoby_do_odpowiedzi;

public partial class ManageStudentsPage : ContentPage
{
	public ManageStudentsPage()
	{
		InitializeComponent();
		BindingContext = new ManageStudentsViewModel();
	}
}