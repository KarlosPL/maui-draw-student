using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace System_losowania_osoby_do_odpowiedzi.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task ManageClasses()
        {
            await Shell.Current.GoToAsync(nameof(ManageClassesPage));
        }

        [RelayCommand]
        public async Task ShowClassList()
        {
            await Shell.Current.GoToAsync(nameof(ClassListPage));
        }
    }
}
