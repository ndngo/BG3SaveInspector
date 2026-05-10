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
        private bool _isCompleted;

        public string ObjectiveId { get => _objectiveId; set { _objectiveId = value; OnPropertyChanged(); } }
        public string StepId { get => _stepId; set { _stepId = value; OnPropertyChanged(); } }
        public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged();} }
    }
}
