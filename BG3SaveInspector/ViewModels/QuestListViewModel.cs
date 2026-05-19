using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LSLib.LS;
using System.ComponentModel;
using System.Windows.Data;
using BG3SaveInspector.Services;
using System.Windows.Input;
using BG3SaveInspector.Commands;

namespace BG3SaveInspector.ViewModels
{
    public class QuestListViewModel : BaseViewModel
    {
        private ObservableCollection<QuestItemViewModel> _quests { get; } = new();
        
        private QuestItemViewModel _selectedQuest;
        private string _searchText;
        public ICollectionView QuestsView { get; }
        public ICommand ClearSearchCommand { get; }
        public int QuestCount => _quests.Count;
        
        public QuestListViewModel()
        {
            QuestsView = CollectionViewSource.GetDefaultView(_quests);
            QuestsView.Filter = FilterQuest;
            ClearSearchCommand = new RelayCommand(() => SearchText = string.Empty);
        }

        private bool FilterQuest(object obj)
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return true;
            }

            var quest = (QuestItemViewModel)obj;
            return quest.ObjectiveId.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        public QuestItemViewModel SelectedQuest
        {
            get => _selectedQuest;
            set
            {
                _selectedQuest = value;
                OnPropertyChanged();
                if (value != null)
                {
                    QuestSelected?.Invoke(value);
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                QuestsView.Refresh();
            }
        }

        public event Action<QuestItemViewModel>? QuestSelected;


        public void Populate(Resource resource)
        {
            _quests.Clear();
            var quests = SaveFileParser.ParseQuestsFromResource(resource);

            foreach (var q in quests)
            {
                _quests.Add(q);
            }
        }

    }
}
