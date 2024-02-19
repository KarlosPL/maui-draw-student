using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System_losowania_osoby_do_odpowiedzi.Models;

namespace System_losowania_osoby_do_odpowiedzi.ViewModels
{
    public partial class ClassesViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<ClassModel> classes = new();

        [ObservableProperty]
        public string enteredClassName;

        public ClassesViewModel() 
        {
            LoadClasses();
        }

        [RelayCommand]
        public async Task AddNewClass()
        {
            if (EnteredClassName == null)
            {
                return;
            }

            var newClass = new ClassModel { ClassName = EnteredClassName };
            Classes.Add(newClass);
            await SaveClasses();

            EnteredClassName = "";
        }

        [RelayCommand]
        public async Task RemoveClass(ClassModel classToRemove)
        {
            if (classToRemove != null)
            {
                Classes.Remove(classToRemove);
                await SaveClasses();

                var studentsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");

                if (File.Exists(studentsFilePath))
                {
                    var remainingStudents = new List<string>();

                    using (var reader = new StreamReader(studentsFilePath))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            var studentData = line.Split(',');
                            if (studentData[3] != classToRemove.ClassName)
                            {
                                remainingStudents.Add(line);
                            }
                        }
                    }

                    using var writer = new StreamWriter(studentsFilePath, false);
                    foreach (var student in remainingStudents)
                    {
                        await writer.WriteLineAsync(student);
                    }
                }
            }
        }


        [RelayCommand]
        public async Task OpenManageStudents(ClassModel selectedClass)
        {
            if (selectedClass == null) return;
            await Shell.Current.GoToAsync($"{nameof(ManageStudentsPage)}?ClassName={selectedClass.ClassName}");
        }

        private async Task SaveClasses()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "classes.txt");
            using var writer = new StreamWriter(filePath, false);
            foreach (var classModel in Classes)
            {
                await writer.WriteLineAsync(classModel.ClassName);
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
                    Classes.Add(new ClassModel { ClassName = line });
                }
            }
        }
    }
}
