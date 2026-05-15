using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BG3SaveInspector.ViewModels;
using LSLib.LS;

namespace BG3SaveInspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            var reader = new PackageReader();
            string cwd = Directory.GetCurrentDirectory();
            System.Diagnostics.Debug.WriteLine("cwd is: " + cwd);
            var package = reader.Read(@"../../../../../QuickSave_452.lsv");

            PackagedFileInfo globals = package.Files.First(f => f.Name == "Globals.lsf");
            var globalStream = globals.CreateContentReader();
            var streamReader = new LSLib.LS.LSFReader(globalStream);
            var globalResource = streamReader.Read();
            var globalRegions = globalResource.Regions;

            System.Diagnostics.Debug.WriteLine("---Globals---");
            foreach (var globalRegion in globalRegions)
            {
                System.Diagnostics.Debug.WriteLine(globalRegion.Key);
            }



            Region osirisVarHelper = globalResource.Regions["OsirisVariableHelper"];
            List<Node> variableManager = osirisVarHelper.Children["VariableManager"];
            List<Node> flagMap = variableManager[0].Children["FlagMap"];
            List<Node> flagContainerMap = flagMap[0].Children["FlagContainerMap"];
            Dictionary<string, List<Node>> flagContainerMapNode = flagContainerMap[0].Children;
            var flagObject = flagContainerMapNode.FirstOrDefault();
            var flags = flagObject.Value;

            List<Node> variantMap = variableManager[0].Children["VariantMap"];
            var variablesVariant = variantMap[0].Children["VariablesVariant"];

            Region story = globalResource.Regions["Story"];
            var osirisNotificationBuffer = story.Children["Story"][0];

            var file = package.Files.First(f => f.Name == "meta.lsf");
            var metaStream = globals.CreateContentReader();
            streamReader = new LSLib.LS.LSFReader(metaStream);
            var metaResource = streamReader.Read();
            var metaRegions = metaResource.Regions;

            System.Diagnostics.Debug.WriteLine("---Meta---");
            foreach ( var metaRegion in metaRegions )
            {
                System.Diagnostics.Debug.WriteLine(metaRegion.Key);
            }

            Region journal = globalResource.Regions["Journal"];
            Dictionary<string, List<Node>> questsDictionary = journal.Children["Quests"][0].Children;
            List<Node> test1 = journal.Children["Quests"];
            var questsProgress = journal.Children["Quests"][0].Children["Quests"][0].Children["QuestsProgress"];

            System.Diagnostics.Debug.WriteLine("---Quests---");

            NodeSerializationSettings SerializationSettings = new();

            foreach ( var questProgress in questsProgress)
            {
                System.Diagnostics.Debug.WriteLine($"QUEST: {questProgress.Name}------");
                System.Diagnostics.Debug.WriteLine(questProgress.Children["MapValue"][0].Children["Quest"]);
                var questStatus = questProgress.Children["MapValue"][0].Children["Quest"][0];
                

                foreach (var attribute in questStatus.Attributes)
                {
                    System.Diagnostics.Debug.WriteLine($"{attribute.Key}-{attribute.Value.AsString(SerializationSettings)}");
                }
            }
        }
    }
}