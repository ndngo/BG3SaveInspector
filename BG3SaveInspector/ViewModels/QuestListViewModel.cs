using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }
    }
}
