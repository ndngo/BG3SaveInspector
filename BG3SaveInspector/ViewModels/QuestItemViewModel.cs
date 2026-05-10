using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG3SaveInspector.ViewModels
{
    public class QuestItemViewModel : BaseViewModel
    {
        public string ObjectiveId { get; set; }
        public string StepId { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsDisabled { get; set; }
        public string Prefix { get; set; }

        public string DisplayName => ObjectiveId
            .Replace("_COMPLETION", "")
            .Replace("_SUB_", " - ")
            .Replace("_", " ");
        public string Status => IsDisabled ? "Completed" : IsUnlocked ? "Active" : "Locked";
    }
}
