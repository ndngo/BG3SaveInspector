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
using SkiaSharp;
using SkiaSharp.Views.WPF;
using System.Collections.ObjectModel;
using System.Text.Json;

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
        public BitmapImage Thumbnail { get => _thumbnail; set { _thumbnail = value; OnPropertyChanged(); } }
        public string LeaderName { get => _leaderName; set { _leaderName = value; OnPropertyChanged(); } }
        public string LeaderClass { get => _leaderClass; set { _leaderClass = value; OnPropertyChanged(); } }
        public string Difficulty { get => _difficulty; set { _difficulty = value; OnPropertyChanged(); } }
        public string Location { get => _location; set { _location = value; OnPropertyChanged(); } }
        public ObservableCollection<PartyMemberViewModel> Party { get; } = new();
        
        private BitmapImage LoadThumbnail(PackagedFileInfo file)
        {
            using var stream = file.CreateContentReader();
            var bytes = new MemoryStream();
            stream.CopyTo(bytes);
            var byteArray = bytes.ToArray();

            using var skData = SKData.CreateCopy(byteArray);
            using var codec = SKCodec.Create(skData);
            var info = codec.Info;
            using var skBitmap = new SKBitmap(info);
            codec.GetPixels(info, skBitmap.GetPixels());

            var bitmapImage = new BitmapImage();
            using var ms = new MemoryStream(skData.ToArray());
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
        
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

                // get character data
                var characterRegion = resource.Regions["Characters"];

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

                var clientDatas = metaResource.Regions["MetaData"].Children["MetaData"][0].Children["ClientDatas"][0].Children["ClientData"];

                var root = saveInfo.RootElement;
                var characters = root.GetProperty("Active Party").GetProperty("Characters");

                Party.Clear();

                foreach (var character in characters.EnumerateArray())
                {
                    if (!character.TryGetProperty("Origin", out var origin) ||
                        !character.TryGetProperty("Race", out var race) ||
                        !character.TryGetProperty("Level", out var level) ||
                        !character.TryGetProperty("Classes", out var classes)
                        )
                    {
                        continue;
                    }

                    // skip characters with no classes
                    if (classes.GetArrayLength() == 0)
                    {
                        continue;
                    }

                    var classString = string.Join(" / ",
                        classes.EnumerateArray()
                            .Select(c => $"{c.GetProperty("Sub").GetString()} {c.GetProperty("Main").GetString()}")
                            .Select(s => s.Trim())
                    );

                    string originStr = origin.GetString();

                    Party.Add(new PartyMemberViewModel
                    { 
                        Name = originStr == "Generic" ? "Tav" : originStr,
                        Race = race.GetString(),
                        Level = level.GetInt32(),
                        ClassString = classString
                    });
                }
                var difficulty = root.GetProperty("Difficulty")[0].GetString();
                var isHonourMode = (root.GetProperty("Difficulty")[1].GetString() == "RulesetHonour");
                LeaderName = metaResource.Regions["MetaData"].Children["MetaData"][0].Attributes["LeaderName"].Value.ToString();
                Difficulty = (isHonourMode) ? "Honour Mode" : difficulty.Replace("Difficulty", "");
                Location = root.GetProperty("Current Level").GetString();
                SaveName = root.GetProperty("Save Name").GetString();

                var thumbnailFile = package.Files.FirstOrDefault(f => f.Name.EndsWith(".webp", StringComparison.OrdinalIgnoreCase));
                if (thumbnailFile != null)
                {
                   Thumbnail = LoadThumbnail(thumbnailFile);
                }
            }
        }
    }
}
