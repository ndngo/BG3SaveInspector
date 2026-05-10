using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BG3SaveInspector.ViewModels
{
    public class SaveFileViewModel : BaseViewModel
    {
        private string _saveName;
        private string _playtime;
        private BitmapImage _thumbnail;

        public string SaveName { get => _saveName; set { _saveName = value; OnPropertyChanged(); } }
        public string Playtime { get => _playtime;set { _playtime = value; OnPropertyChanged(); } }
        public BitmapImage Thumbmail { get => _thumbnail; set { _thumbnail = value; OnPropertyChanged(); } }
    }
}
