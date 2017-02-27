#region

using System.Collections.Generic;
using System.Linq;
using Saturn72.Common.App;
using Saturn72.Extensions;

#endregion

namespace Calculator.Framework.Apps
{
    public class CalculatorApp : Saturn72AppBase
    {
        private const string AppName = "calculator_server";
        private readonly IEnumerable<IAppVersion> _versions;

        public CalculatorApp() : base(AppName)
        {
            _versions = LoadVersions();
        }

        public override string Name
        {
            get { return AppName; }
        }

        public override IEnumerable<IAppVersion> Versions
        {
            get { return _versions; }
        }


        private IEnumerable<IAppVersion> LoadVersions()
        {
            var versions = new IAppVersion[]
            {
                new CalculatorVersion1()
            };

            Guard.MustFollow(() => versions.Count(v => v.IsLatest) == 1,
                "Multiple versions marked as latest, Or noversion is marked as latest");

            return versions;
        }
    }
}