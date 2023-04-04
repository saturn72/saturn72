
namespace Saturn72.Core.Messaging
{
    public class Saturn72EndpointNameFormatter : IEndpointNameFormatter
    {
        public string Separator => throw new NotImplementedException();

        public string Message<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public string SanitizeName(string name)
        {
            throw new NotImplementedException();
        }

        public string TemporaryEndpoint(string tag)
        {
            throw new NotImplementedException();
        }

        string IEndpointNameFormatter.CompensateActivity<T, TLog>()
        {
            throw new NotImplementedException();
        }

        string IEndpointNameFormatter.Consumer<T>()
        {
            throw new NotImplementedException();
        }

        string IEndpointNameFormatter.ExecuteActivity<T, TArguments>()
        {
            throw new NotImplementedException();
        }

        string IEndpointNameFormatter.Saga<T>()
        {
            throw new NotImplementedException();
        }
    }
}
