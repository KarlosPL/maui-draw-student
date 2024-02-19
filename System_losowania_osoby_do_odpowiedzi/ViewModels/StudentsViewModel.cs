using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System_losowania_osoby_do_odpowiedzi.Models;

namespace System_losowania_osoby_do_odpowiedzi.ViewModels
{
    [QueryProperty(nameof(ClassName), "ClassName")]
    public partial class StudentsViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<StudentModel> students = new();

        [ObservableProperty]
        public string enteredFirstName;

        [ObservableProperty]
        public string enteredLastName;

        [ObservableProperty]
        public string className;

        public StudentsViewModel()
        {
            LoadStudents();
        }

        [RelayCommand]
        public async Task AddNewStudent()
        {
            if (string.IsNullOrWhiteSpace(EnteredFirstName) || string.IsNullOrWhiteSpace(EnteredLastName) || string.IsNullOrWhiteSpace(ClassName))
            {
                await Application.Current.MainPage.DisplayAlert("Ostrzeżenie", "Wszystkie pola muszą być wypełnione!", "OK");
                return;
            }

            bool studentExists = Students.Any(s => s.FirstName == EnteredFirstName
                                                && s.LastName == EnteredLastName
                                                && s.AssignedClass == ClassName);

            if (!studentExists)
            {
                var newStudent = new StudentModel
                {
                    FirstName = EnteredFirstName,
                    LastName = EnteredLastName,
                    AssignedClass = ClassName,
                };

                Students.Add(newStudent);
                await SaveStudent(newStudent);
                MessagingCenter.Send(this, "StudentAdded");

                EnteredFirstName = "";
                EnteredLastName = "";
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ostrzeżenie!", "Taki uczeń już jest wpisany w tej klasie", "OK");
            }
        }

        private async Task SaveStudent(StudentModel student)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");
            using var writer = new StreamWriter(filePath, true);
            var line = $"{student.Id},{student.FirstName},{student.LastName},{student.AssignedClass},{student.IsPresent},{student.DrawCount}";
            await writer.WriteLineAsync(line);
        }

        private async Task LoadStudents()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 6 && int.TryParse(parts[0], out int id) && bool.TryParse(parts[4], out bool isPresent) && int.TryParse(parts[5], out int drawCount))
                    {
                        var student = new StudentModel
                        {
                            Id = id,
                            FirstName = parts[1],
                            LastName = parts[2],
                            AssignedClass = parts[3],
                            IsPresent = isPresent,
                            DrawCount = drawCount
                        };

                        Students.Add(student);
                    }
                }
            }
        }
    }
}
