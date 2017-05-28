
using Saturn72.Core.Extensibility;

namespace Saturn72.Modules.MongoDb
{
    public class MongoDbModule:IModule
    {
        private const string StartMongoDCommandFormat = @"cmd.exe /c json-server --port {0}";
        public void Load()
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
