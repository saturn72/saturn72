namespace Saturn72.Core.Extensibility
{
    public interface IPlugin
    {
        void Install();

        void Uninstall();

        void Suspend();

        void Activate();
    }
}