using System_losowania_osoby_do_odpowiedzi.Models;
using System_losowania_osoby_do_odpowiedzi.ViewModels;

namespace System_losowania_osoby_do_odpowiedzi;

public partial class ClassListPage : ContentPage
{
	public ClassListPage()
	{
		InitializeComponent();
		BindingContext = new ClassListViewModel();
	}

    private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is StudentModel student)
        {
            student.IsPresent = checkBox.IsChecked;

            if (BindingContext is ClassListViewModel viewModel)
            {
                viewModel.SaveStudent(student);
            }
        }
    }

}