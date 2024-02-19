using CommunityToolkit.Mvvm.ComponentModel;

namespace System_losowania_osoby_do_odpowiedzi.Models
{
    public partial class LuckyNumberModel : ObservableObject
    {
        [ObservableProperty]
        public DateTime date;

        [ObservableProperty]
        public string luckyNumber;

        public LuckyNumberModel()
        {
            LoadLuckyNumber();
        }

        private void GenerateAndSaveNewLuckyNumber()
        {
            Random random = new();
            LuckyNumber = random.Next(1, 41).ToString();

            SaveLuckyNumber();
        }

        public async void SaveLuckyNumber()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "luckynumber.txt");
            using var writer = new StreamWriter(filePath, false);

            var line = $"{DateTime.Today.ToShortDateString()},{LuckyNumber}";
            await writer.WriteLineAsync(line);
        }

        public void LoadLuckyNumber()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "luckynumber.txt");
            if (File.Exists(filePath))
            {
                var lastLine = File.ReadLines(filePath).LastOrDefault();
                if (lastLine != null)
                {
                    var parts = lastLine.Split(',');
                    if (parts.Length == 2 && DateTime.TryParse(parts[0], out DateTime savedDate))
                    {
                        if (savedDate.Date == DateTime.Today)
                        {
                            LuckyNumber = parts[1];
                            return;
                        }
                    }
                }
            }

            GenerateAndSaveNewLuckyNumber();
        }
    }
}
