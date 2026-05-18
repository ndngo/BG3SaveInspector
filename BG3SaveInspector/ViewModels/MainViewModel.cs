using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LSLib.LS;
using System.Windows.Input;
using BG3SaveInspector.Commands;

namespace BG3SaveInspector.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _statusText = "No save loaded";
        public SaveFileViewModel SaveFile { get; } = new();
        public QuestListViewModel QuestList { get; } = new();
        public QuestDetailViewModel QuestDetail { get; } = new();
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }

        public DiffViewModel Diff { get; } = new();
        // Toggle
        public bool IsDiffMode
        {
            get => _isDiffMode;
            set
            {
                _isDiffMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DiffToggleLabel));
            }
        }
        private bool _isDiffMode;
        public string DiffToggleLabel => IsDiffMode ? "Exit Compare" : "Compare Saves";
        public ICommand ToggleDiffModeCommand { get; }

        public MainViewModel()
        {
            SaveFile = new SaveFileViewModel();
            QuestList = new QuestListViewModel();
            QuestDetail = new QuestDetailViewModel();

            SaveFile.SaveLoaded += resource =>
            {
                QuestList.Populate(resource);
                StatusText =$"Loaded {QuestList.QuestCount} "+ $"{(QuestList.QuestCount > 1 ? "quests" : "quest")}";
            };

            QuestList.QuestSelected += quest => QuestDetail.ShowQuest(quest);
            ToggleDiffModeCommand = new RelayCommand(() => IsDiffMode = !IsDiffMode);
        }

    }


}
