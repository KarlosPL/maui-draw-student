using System_losowania_osoby_do_odpowiedzi.ViewModels;

namespace System_losowania_osoby_do_odpowiedzi;

public partial class AddNewStudentPage : ContentPage
{
	public AddNewStudentPage()
	{
		InitializeComponent();
		BindingContext = new StudentsViewModel();
	}
}