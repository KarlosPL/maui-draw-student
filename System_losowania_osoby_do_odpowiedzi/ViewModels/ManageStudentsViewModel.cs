using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System_losowania_osoby_do_odpowiedzi.Models;

namespace System_losowania_osoby_do_odpowiedzi.ViewModels
{
    [QueryProperty(nameof(ClassName), "ClassName")]
    public partial class ManageStudentsViewModel : ObservableObject
    {
        [ObservableProperty]
        public string className;

        [ObservableProperty]
        private ObservableCollection<StudentModel> students = new();

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        public ManageStudentsViewModel()
        {
            LoadStudents();

            MessagingCenter.Subscribe<StudentsViewModel>(this, "StudentAdded", (sender) =>
            {
                LoadStudents();
            });
        }

        [RelayCommand]
        public async Task AddStudent()
        {
            await Shell.Current.GoToAsync($"{nameof(AddNewStudentPage)}?ClassName={ClassName}");
            await LoadStudents();
        }

        public async Task LoadStudents()
        {
            Students.Clear();
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");
            List<StudentModel> tempStudents = new();

            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 6 && int.TryParse(parts[0], out int id) && bool.TryParse(parts[4], out bool isPresent) && int.TryParse(parts[5], out int drawCount))
                    {
                        if (parts[3] == ClassName)
                        {
                            tempStudents.Add(new StudentModel
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
            }

            var sortedStudents = tempStudents.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();

            for (int i = 0; i < sortedStudents.Count; i++)
            {
                sortedStudents[i].Number = i + 1;
                Students.Add(sortedStudents[i]);
            }
        }

        [RelayCommand]
        public async Task EditStudent(StudentModel student)
        {
            if (student == null) return;

            var existingStudent = Students.FirstOrDefault(s => s.FirstName == student.FirstName && s.LastName == student.LastName && s.AssignedClass == student.AssignedClass);
            if (existingStudent != null)
            {
                existingStudent.FirstName = student.FirstName;
                existingStudent.LastName = student.LastName;
            }

            await SaveStudents();
            await LoadStudents();
        }

        [RelayCommand]
        public async Task DeleteStudent(StudentModel student)
        {
            if (student == null) return;

            Students.Remove(student);
            await SaveStudents(student);
            await LoadStudents();
        }

        private async Task SaveStudents(StudentModel deletedStudent = null)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");

            var allStudents = new ObservableCollection<StudentModel>();
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
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

            if (deletedStudent != null)
            {
                var studentToRemove = allStudents.FirstOrDefault(s => s.Id == deletedStudent.Id);
                if (studentToRemove != null)
                {
                    allStudents.Remove(studentToRemove);
                }

            }
            else
            {
                foreach (var student in Students)
                {
                    var existingStudent = allStudents.FirstOrDefault(s => s.Id == student.Id);
                    if (existingStudent != null)
                    {
                        allStudents.Remove(existingStudent);
                    }
                    allStudents.Add(student);
                }
            }

            using var writer = new StreamWriter(filePath, false);
            foreach (var student in allStudents)
            {
                await writer.WriteLineAsync($"{student.Id},{student.FirstName},{student.LastName},{student.AssignedClass},{student.IsPresent},{student.DrawCount}");
            }
        }
    }
}
