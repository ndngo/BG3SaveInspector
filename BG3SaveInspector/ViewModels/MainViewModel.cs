using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BG3SaveInspector.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SaveFileViewModel SaveFile { get; } = new();
        public QuestListViewModel QuestList { get; } = new();
        public QuestDetailViewModel QuestDetail { get; } = new();
    }
}
