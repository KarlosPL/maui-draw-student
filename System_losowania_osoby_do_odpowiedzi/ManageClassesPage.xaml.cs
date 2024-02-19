using System_losowania_osoby_do_odpowiedzi.ViewModels;

namespace System_losowania_osoby_do_odpowiedzi;

public partial class ManageClassesPage : ContentPage
{
	public ManageClassesPage()
	{
		InitializeComponent();
		BindingContext = new ClassesViewModel();
	}
}