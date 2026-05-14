using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Input;
using BG3SaveInspector.Commands;
using System.Security.Cryptography.Xml;
using System.IO;
using System.IO.Packaging;
using LSLib.LS;

namespace BG3SaveInspector.ViewModels
{
    public class SaveFileViewModel : BaseViewModel
    {
        private string _saveName;
        private string _playtime;
        private BitmapImage _thumbnail;
        private string _leaderName;
        private string _leaderClass;
        private string _difficulty;
        private string _location;

        public ICommand LoadCommand { get; }

        public event Action<Resource>? SaveLoaded;


        public SaveFileViewModel()
        {
            LoadCommand = new RelayCommand(LoadSave);
        }

        public string SaveName { get => _saveName; set { _saveName = value; OnPropertyChanged(); } }
        public string Playtime { get => _playtime;set { _playtime = value; OnPropertyChanged(); } }
        public BitmapImage Thumbmail { get => _thumbnail; set { _thumbnail = value; OnPropertyChanged(); } }
        public string LeaderName { get => _leaderName; set { _leaderName = value; OnPropertyChanged(); } }
        public string LeaderClass { get => _leaderClass; set { _leaderClass = value; OnPropertyChanged(); } }
        public string Difficulty { get => _difficulty; set { _difficulty = value; OnPropertyChanged(); } }
        public string Location { get => _location; set { _location = value; OnPropertyChanged(); } }
        private void LoadSave()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open BG3 File Save",
                Filter = "BG3 Sav Files (*.lsv)|*.lsv",
                InitialDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Larian Studios\Baldur's Gate 3\PlayerProfiles\Public\Savegames\Story")
            };

            if (dialog.ShowDialog() == true)
            {
                var reader = new PackageReader();
                var package = reader.Read(dialog.FileName);

                // get story flags
                var globalsFile = package.Files.First(f => f.Name == "Globals.lsf");
                var stream = globalsFile.CreateContentReader();
                var lsfReader = new LSFReader(stream);
                var resource = lsfReader.Read();
                SaveName = Path.GetFileNameWithoutExtension(dialog.FileName);
                var journalRegion = resource.Regions["Journal"];
                SaveLoaded?.Invoke(resource);

                // get party data
                var saveInfoFile = package.Files.First(f => f.Name == "SaveInfo.json");
                var saveInfoStream = saveInfoFile.CreateContentReader();
                using var reader2 = new StreamReader(saveInfoStream);
                var json = reader2.ReadToEnd();
                var saveInfo = System.Text.Json.JsonDocument.Parse(json);

                // get metadata
                var metaFile = package.Files.First(f => f.Name == "meta.lsf");
                var metaStream = metaFile.CreateContentReader();
                var metaReader = new LSFReader(metaStream);
                var metaResource = metaReader.Read();
                var leaderName = metaResource.Regions["MetaData"].Children["MetaData"][0].Attributes["LeaderName"].Value.ToString();

                var root = saveInfo.RootElement;
                var characters = root.GetProperty("Active Party").GetProperty("Characters");
                var mc = characters.EnumerateArray().First(c => c.GetProperty("Origin").GetString() == leaderName);

                // MC class string
                var classes = mc.GetProperty("Classes");
                var classString = string.Join(" / ",
                    Enumerable.Range(0, classes.GetArrayLength())
                        .Select(i => $"{classes[i].GetProperty("Sub").GetString()} {classes[i].GetProperty("Main").GetString()}")
                );

                var level = mc.GetProperty("Level").GetInt32();
                var difficulty = root.GetProperty("Difficulty")[1].GetString();

                LeaderName = metaResource.Regions["MetaData"].Children["MetaData"][0].Attributes["LeaderName"].Value.ToString();
                LeaderClass = $"Lv.{level} {classString}";
                Difficulty = difficulty.Replace("Ruleset", "").Replace("Honour", "Honour Mode").Replace("Larian", "Balanced");
                Location = root.GetProperty("Current Level").GetString();
                SaveName = root.GetProperty("Save Name").GetString();

            }
        }
    }
}
