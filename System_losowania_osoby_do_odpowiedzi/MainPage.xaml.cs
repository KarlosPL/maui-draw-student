using System_losowania_osoby_do_odpowiedzi.ViewModels;

namespace System_losowania_osoby_do_odpowiedzi
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }

}
