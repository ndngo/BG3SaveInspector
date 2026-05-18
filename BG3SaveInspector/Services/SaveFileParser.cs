using BG3SaveInspector.ViewModels;
using LSLib.LS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BG3SaveInspector.Services
{
    public static class SaveFileParser
    {
        public static List<QuestItemViewModel> ParseQuests(string path)
        {
            var reader = new PackageReader();
            var package = reader.Read(path);
            var globalsFile = package.Files.First(f => f.Name == "Globals.lsf");
            var stream = globalsFile.CreateContentReader();
            var lsfReader = new LSFReader(stream);
            var resource = lsfReader.Read();

            return ParseQuestsFromResource(resource);
        }

        public static List<QuestItemViewModel> ParseQuestsFromResource(Resource resource)
        {
            var progressNodes = resource?.Regions["Journal"]
                ?.Children["Quests"][0]
                ?.Children["Quests"][0]
                ?.Children["QuestsProgress"];

            if (progressNodes == null || !progressNodes.Any())
            {
                return new List<QuestItemViewModel>();
            }

            var quests = new List<QuestItemViewModel>();
            foreach (var node in progressNodes)
            {
                var attributes = node.Children["MapValue"][0].Children["Quest"][0].Attributes;

                var objectiveId = attributes["ObjectiveID"].Value.ToString();
                var stepId = attributes["UnlockedByStepID"].Value.ToString();
                var isUnlocked = (bool)attributes["QuestUnlocked"].Value;
                var isDisabled = (bool)attributes["QuestDisabled"].Value;
                var prefix = objectiveId.Contains("_") ? objectiveId.Split('_')[0] : "OTHER"; 

                quests.Add(new QuestItemViewModel
                {
                    ObjectiveId = objectiveId,
                    StepId = stepId,
                    IsUnlocked = isUnlocked,
                    IsDisabled = isDisabled,
                    Prefix = prefix
                });
            }

            return quests;
        }
    }
}
