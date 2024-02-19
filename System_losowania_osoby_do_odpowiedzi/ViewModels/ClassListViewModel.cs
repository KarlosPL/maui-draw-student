using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System_losowania_osoby_do_odpowiedzi.Models;

namespace System_losowania_osoby_do_odpowiedzi.ViewModels
{
    public partial class ClassListViewModel : ObservableObject
    {
        private static readonly object FileLock = new();

        [ObservableProperty]
        private ObservableCollection<string> classes = new();

        [ObservableProperty]
        private string selectedClass;

        [ObservableProperty]
        private ObservableCollection<StudentModel> students = new();

        [ObservableProperty]
        private ObservableCollection<StudentModel> filteredStudents = new();

        [ObservableProperty]
        private string displayedClass;

        [ObservableProperty]
        private string randomStudent;

        [ObservableProperty]
        private string luckyNumberDisplay;

        public ClassListViewModel() 
        {
            LoadClasses();
            LoadStudents();
            LuckyNumber();
        }

        private void LuckyNumber()
        {
            var luckyNumber = new LuckyNumberModel();
            LuckyNumberDisplay = luckyNumber.LuckyNumber;
        }

        [RelayCommand]
        public void DrawStudent()
        {
            if (!FilteredStudents.Any())
            {
                Application.Current.MainPage.DisplayAlert("Ostrzeżenie", "Najpierw wybierz klasę i wyświetl listę uczniów", "OK");
                return;
            }

            foreach (var student in FilteredStudents)
            {
                if (student.DrawCount > 3 && student.IsPresent)
                {
                    student.DrawCount = 0;
                    SaveStudent(student);
                }

                if (student.DrawCount > 0 && student.IsPresent)
                {
                    student.DrawCount++;
                    SaveStudent(student);
                }
            }

            var studentsToDrawFrom = FilteredStudents
                .Where(student => student.Number.ToString() != LuckyNumberDisplay
                                  && student.IsPresent
                                  && student.DrawCount == 0)
                .ToList();

            if (!studentsToDrawFrom.Any())
            {
                Application.Current.MainPage.DisplayAlert("Informacja", "Brak obecnych uczniów do losowania lub wszyscy byli już pytani", "OK");
                return;
            }

            var random = new Random();
            var randomIndex = random.Next(0, studentsToDrawFrom.Count);
            var selectedStudent = studentsToDrawFrom[randomIndex];

            selectedStudent.DrawCount++;
            SaveStudent(selectedStudent);

            RandomStudent = $"{selectedStudent.FirstName} {selectedStudent.LastName}";
        }


        [RelayCommand]
        public void ShowList()
        {
            if (string.IsNullOrWhiteSpace(SelectedClass))
            {
                Application.Current.MainPage.DisplayAlert("Ostrzeżenie", "Klasa musi być wybrana", "OK");
                return;
            }

            DisplayedClass = SelectedClass;

            var sortedFilteredStudents = Students
                .Where(student => student.AssignedClass == SelectedClass)
                .OrderBy(student => student.FirstName)
                .ThenBy(student => student.LastName)
                .ToList();

            for (int i = 0; i < sortedFilteredStudents.Count; i++)
            {
                sortedFilteredStudents[i].Number = i + 1;
            }

            FilteredStudents = new ObservableCollection<StudentModel>(sortedFilteredStudents);
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

        public void SaveStudent(StudentModel student)
        {
            lock (FileLock)
            {
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");
                var allStudents = new List<StudentModel>();

                if (File.Exists(filePath))
                {
                    using var reader = new StreamReader(filePath);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 6 && int.TryParse(parts[0], out int id) && bool.TryParse(parts[4], out bool isPresent) && int.TryParse(parts[5], out int drawCount))
                        {
                            allStudents.Add(new StudentModel
                            {
                                Id = id,
                                FirstName = parts[1],
                                LastName = parts[2],
                                AssignedClass = parts[3],
                                IsPresent = isPresent,
                                DrawCount = drawCount
                            });
                        }
                    }
                }

                var existingStudent = allStudents.FirstOrDefault(s => s.Id == student.Id);
                if (existingStudent != null)
                {
                    existingStudent.IsPresent = student.IsPresent;
                    existingStudent.DrawCount = student.DrawCount;
                }

                using var writer = new StreamWriter(filePath, false);
                foreach (var s in allStudents)
                {
                    writer.WriteLine($"{s.Id},{s.FirstName},{s.LastName},{s.AssignedClass},{s.IsPresent},{s.DrawCount}");
                }
            }
        }


        private async Task LoadClasses()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "classes.txt");
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Classes.Add(line);
                }
            }
        }
    }
}
