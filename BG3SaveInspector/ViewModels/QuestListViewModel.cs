using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LSLib.LS;

namespace BG3SaveInspector.ViewModels
{
    public class QuestListViewModel : BaseViewModel
    {
        public ObservableCollection<QuestItemViewModel> Quests { get; } = new();
        private QuestItemViewModel _selectedQuest;
        public QuestItemViewModel SelectedQuest { get => _selectedQuest; set { _selectedQuest = value; OnPropertyChanged(); } }

        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

        public void Populate(Resource resource)
        {
            Quests.Clear();

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

                Quests.Add(new QuestItemViewModel
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
