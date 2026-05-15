using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG3SaveInspector.ViewModels
{
    public class PartyMemberViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Origin { get; set; }
        public string Race { get; set; }
        public string ClassString { get; set; }
        public int Level { get; set; }

        public string DisplayName => $"{Name} - Lv.{Level} {ClassString}";
    }
}
