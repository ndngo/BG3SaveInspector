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
        public ICommand LoadCommand { get; }

        public event Action<Resource>? SaveLoaded;


        public SaveFileViewModel()
        {
            LoadCommand = new RelayCommand(LoadSave);
        }

        public string SaveName { get => _saveName; set { _saveName = value; OnPropertyChanged(); } }
        public string Playtime { get => _playtime;set { _playtime = value; OnPropertyChanged(); } }
        public BitmapImage Thumbmail { get => _thumbnail; set { _thumbnail = value; OnPropertyChanged(); } }

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
                var globalsFile = package.Files.First(f => f.Name == "Globals.lsf");
                var stream = globalsFile.CreateContentReader();
                var lsfReader = new LSFReader(stream);
                var resource = lsfReader.Read();
                SaveName = Path.GetFileNameWithoutExtension(dialog.FileName);
                var journalRegion = resource.Regions["Journal"];
                SaveLoaded?.Invoke(resource);
            }
        }
    }
}
