using BG3SaveInspector.Commands;
using BG3SaveInspector.Models;
using BG3SaveInspector.Services;
using LSLib.LS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace BG3SaveInspector.ViewModels
{
    public class DiffViewModel : BaseViewModel
    {
        private string _saveAName;
        private string _saveBName;
        private string _diffSummary;
        private string _searchTextA;
        private string _searchTextB;

        public string SaveAName { get => _saveAName; set { _saveAName = value;  OnPropertyChanged(); } }
        public string SaveBName { get => _saveBName; set { _saveBName = value; OnPropertyChanged(); } }
        public string DiffSummary { get => _diffSummary; set { _diffSummary = value; OnPropertyChanged(); } }
        public string SearchTextA
        {
            get => _searchTextA;
            set
            {
                _searchTextA = value;
                OnPropertyChanged();
                SaveAQuestsView.Refresh();
            }
        }

        public string SearchTextB
        {
            get => _searchTextB;
            set
            {
                _searchTextB = value;
                OnPropertyChanged();
                SaveBQuestsView.Refresh();
            }
        }

        public ICollectionView SaveAQuestsView { get; }
        public ICollectionView SaveBQuestsView { get; }
        public ObservableCollection<QuestItemViewModel> SaveAQuests { get; } = new();
        public ObservableCollection<QuestItemViewModel> SaveBQuests { get; } = new();
        public ICommand LoadSaveACommand { get; }
        public ICommand LoadSaveBCommand { get; }
        
        public DiffViewModel()
        {
            LoadSaveACommand = new RelayCommand(() => LoadSave(isSaveA: true));
            LoadSaveBCommand = new RelayCommand(() => LoadSave(isSaveA: false));

            SaveAQuestsView = CollectionViewSource.GetDefaultView(SaveAQuests);
            SaveBQuestsView = CollectionViewSource.GetDefaultView(SaveBQuests);

            SaveAQuestsView.Filter = obj => FilterQuest(obj, SearchTextA);
            SaveBQuestsView.Filter = obj => FilterQuest(obj, SearchTextB);
            
        }

        private bool FilterQuest(object obj, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }

            var quest = (QuestItemViewModel)obj;
            return quest.ObjectiveId.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        private void LoadSave(bool isSaveA)
        {
            var dialog = new OpenFileDialog
            {
                Title = isSaveA ? "Open Save A" : "Open Save B",
                Filter = "BG3 Save Files (*.lsv)|*.lsv",
                InitialDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Larian Studios\Baldur's Gate 3\PlayerProfiles\Public\Savegames\Story"
                )
            };

            if (dialog.ShowDialog() == true)
            {
                var quests = SaveFileParser.ParseQuests(dialog.FileName);

                if (isSaveA)
                {
                    SaveAQuests.Clear();
                    foreach (var q in quests)
                    {
                        SaveAQuests.Add(q);
                    }
                    SaveAName = Path.GetFileNameWithoutExtension(dialog.FileName);
                }
                else
                {
                    SaveBQuests.Clear();
                    foreach(var q in quests)
                    {
                        SaveBQuests.Add(q);
                        SaveBName = Path.GetFileNameWithoutExtension(dialog.FileName);
                    }
                }

                if (SaveAQuests.Any() && SaveBQuests.Any())
                {
                    RunDiff();
                }
            }
        }

        private void RunDiff()
        {
            //define objcetiveID as the key
            var saveADict = SaveAQuests.ToDictionary(q => q.ObjectiveId);
            var saveBDict = SaveBQuests.ToDictionary(q => q.ObjectiveId);

            // quests that are in B but not in A
            var added = saveBDict.Keys.Except(saveADict.Keys).ToHashSet();

            // quests that are in A but not in B
            var removed = saveADict.Keys.Except(saveBDict.Keys).ToHashSet();

            var inBoth = saveADict.Keys.Intersect(saveBDict.Keys)
                .Where(id => saveADict[id].Status != saveBDict[id].Status ||
                            saveADict[id].StepId != saveBDict[id].StepId)
                .ToHashSet();

            foreach (var quest in SaveAQuests)
            {
                quest.DiffStatus = removed.Contains(quest.ObjectiveId) ? DiffStatus.Removed :
                    inBoth.Contains(quest.ObjectiveId) ? DiffStatus.Modified :
                    DiffStatus.Unchanged;
            }

            foreach (var quest in SaveBQuests)
            {
                quest.DiffStatus = added.Contains(quest.ObjectiveId) ? DiffStatus.Added :
                    inBoth.Contains(quest.ObjectiveId) ? DiffStatus.Modified :
                    DiffStatus.Unchanged;
            }

            var addedCount = added.Count;
            var removedCount = removed.Count;
            var modifiedCount = inBoth.Count;
            DiffSummary = $"{addedCount} added · {removedCount} removed · {modifiedCount} · modified";
        }
    }
}
