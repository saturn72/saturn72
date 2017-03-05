#region

using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Common.App;

#endregion

namespace Calculator.Framework.Apps
{
    public class CalculatorApp : Saturn72AppBase
    {
        private const string AppName = "calculator_server";

        public CalculatorApp() : base(AppName)
        {
            Versions = LoadVersions();
        }

        public override string Name
        {
            get { return AppName; }
        }

        public override IEnumerable<IAppVersion> Versions { get; }


        private IEnumerable<IAppVersion> LoadVersions()
        {
            var versions = new IAppVersion[]
            {
                new CalculatorVersion1()
            };

            if (versions.Count(v => v.IsLatest) > 1)
                throw new ArgumentException("Multiple versions marked as latest, Or noversion is marked as latest");

            return versions;
        }
    }
}