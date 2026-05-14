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

namespace BG3SaveInspector.ViewModels
{
    public class QuestListViewModel : BaseViewModel
    {
        private ObservableCollection<QuestItemViewModel> _quests { get; } = new();
        public ICollectionView QuestsView { get; }
        private QuestItemViewModel _selectedQuest;
        private string _searchText;

        public QuestListViewModel()
        {
            QuestsView = CollectionViewSource.GetDefaultView(_quests);
            QuestsView.Filter = FilterQuest;
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

            System.Diagnostics.Debug.WriteLine("---POPULATE()---");

            if (!resource.Regions.ContainsKey("Journal"))
            {
                return;
            }
            
            var journal = resource.Regions["Journal"];
            
            if (!journal.Children.ContainsKey("Quests"))
            {
                return;
            }

            var questsNode = journal.Children["Quests"].First();

            System.Diagnostics.Debug.WriteLine($"Reading in quests...");
            var progressNodes = journal.Children["Quests"][0].Children["Quests"][0].Children["QuestsProgress"];
            NodeSerializationSettings SerializationSettings = new();

            foreach (var node in progressNodes)
            {
                var questStatus = node.Children["MapValue"][0].Children["Quest"][0];
                var attributes = questStatus.Attributes;

                var objectiveId = attributes["ObjectiveID"].Value.ToString();
                var stepId = attributes["UnlockedByStepID"].Value.ToString();
                var isUnlocked = (bool)attributes["QuestUnlocked"].Value;
                var isDisabled = (bool)attributes["QuestDisabled"].Value;

                // get quest flags

                // flag category
                var prefix = objectiveId.Contains("_")
                    ? objectiveId.Split('_')[0]
                    : "OTHER";

                _quests.Add(new QuestItemViewModel
                {
                    ObjectiveId = objectiveId,
                    StepId = stepId,
                    IsUnlocked = isUnlocked,
                    IsDisabled = isDisabled,
                    Prefix= prefix
                });

                System.Diagnostics.Debug.WriteLine($"NODE: {node.Name}\nobjective: {objectiveId}\nstepID: {stepId}\nunlocked?: {isUnlocked}\ndisabled?: {isDisabled}");
            }
        }
    }
}
