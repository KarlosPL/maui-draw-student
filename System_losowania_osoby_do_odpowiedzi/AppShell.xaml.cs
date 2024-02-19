namespace System_losowania_osoby_do_odpowiedzi
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddNewStudentPage), typeof(AddNewStudentPage));
            Routing.RegisterRoute(nameof(ManageClassesPage), typeof(ManageClassesPage));
            Routing.RegisterRoute(nameof(ClassListPage), typeof(ClassListPage));
            Routing.RegisterRoute(nameof(ManageStudentsPage), typeof(ManageStudentsPage));
        }
    }
}
