using System;
using Saturn72.Core.Services.Impl.Extensibility;

namespace DummyPlugin1
{
    public class Plugin : PluginBase<DummyPlugin1Settings>
    {
        public override DummyPlugin1Settings DefaultSettings { get; }
        public override void Suspend()
        {
            throw new NotImplementedException();
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }
    }
}
