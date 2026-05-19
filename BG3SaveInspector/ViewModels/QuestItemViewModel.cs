using BG3SaveInspector.Models;
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
        public int DiffSortOrder => DiffStatus switch
        {
            DiffStatus.Modified => 0,
            DiffStatus.Added => 1,
            DiffStatus.Removed => 2,
            DiffStatus.Unchanged => 3,
            _ => 4
        };
        public string DisplayName => ObjectiveId
            .Replace("_COMPLETION", "")
            .Replace("_SUB_", " - ")
            .Replace("_", " ");
        public string Status => IsDisabled ? "Completed" : IsUnlocked ? "Active" : "Locked";
        private DiffStatus _diffStatus = DiffStatus.Unchanged;
        public DiffStatus DiffStatus
        {
            get => _diffStatus;
            set
            {
                _diffStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DiffColour));
                OnPropertyChanged(nameof(DiffSortOrder));
            }
        }
        public System.Windows.Media.Brush DiffColour => DiffStatus switch
        {
            DiffStatus.Added => System.Windows.Media.Brushes.Green,
            DiffStatus.Removed => System.Windows.Media.Brushes.Red,
            DiffStatus.Modified => System.Windows.Media.Brushes.DarkGoldenrod,
            _ => System.Windows.Media.Brushes.Transparent
        };
    }
}
