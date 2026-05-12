using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LSLib.LS;

namespace BG3SaveInspector.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SaveFileViewModel SaveFile { get; } = new();
        public QuestListViewModel QuestList { get; } = new();
        public QuestDetailViewModel QuestDetail { get; } = new();
        public MainViewModel()
        {
            SaveFile = new SaveFileViewModel();
            QuestList = new QuestListViewModel();
            QuestDetail = new QuestDetailViewModel();

            SaveFile.SaveLoaded += resource => QuestList.Populate(resource);
        }

    }


}
