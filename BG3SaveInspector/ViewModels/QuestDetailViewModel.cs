using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG3SaveInspector.ViewModels
{
    public class QuestDetailViewModel : BaseViewModel
    {
        private string _objectiveId;
        private string _stepId;
        private string _status;
        private bool _isCompleted;
        private bool _hasSelection;

        public string ObjectiveId { get => _objectiveId; set { _objectiveId = value; OnPropertyChanged(); } }
        public string StepId { get => _stepId; set { _stepId = value; OnPropertyChanged(); } }
        public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged();} }
        public string Status { get => _status; set {  _status = value; OnPropertyChanged(); } }
        public bool HasSelection { get => _hasSelection; set {  _hasSelection = value; OnPropertyChanged(); } }

        public void ShowQuest(QuestItemViewModel quest)
        {
            ObjectiveId = quest.ObjectiveId;
            StepId = quest.StepId;
            Status = quest.Status;
            HasSelection = _hasSelection;
        }
    }
}
