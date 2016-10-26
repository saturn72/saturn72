using Saturn72.Core.Configuration;

namespace Saturn72.Core.Domain.Tasks
{
    public class BackgroundTaskSettings:SettingsBase
    {
        public bool SaveToDatabase { get; set; }

        public string RootSavePath { get; set; }

        public bool CompressAllAttachtmentsToPackage { get; set; }
    }
}
