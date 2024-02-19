using CommunityToolkit.Mvvm.ComponentModel;

namespace System_losowania_osoby_do_odpowiedzi.Models
{
    public partial class StudentModel : ObservableObject
    {
        [ObservableProperty]
        public int id;

        [ObservableProperty]
        public string firstName;

        [ObservableProperty] 
        public string lastName;

        [ObservableProperty]
        public string assignedClass;

        [ObservableProperty]
        private int number;

        [ObservableProperty]
        private bool isPresent = true;

        [ObservableProperty]
        private int drawCount = 0;

        public StudentModel()
        {
            Id = GenerateUniqueId();
            FirstName = "";
            LastName = "";
            AssignedClass = "";
        }

        public StudentModel(string firstName, string lastName, string assignedClass)
        {
            Id = GenerateUniqueId();
            FirstName = firstName;
            LastName = lastName;
            AssignedClass = assignedClass;
        }

        private static int GenerateUniqueId()
        {
            return (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
        }

    }
}
