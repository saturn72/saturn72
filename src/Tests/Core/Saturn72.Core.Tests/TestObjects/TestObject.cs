#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Tests.TestObjects
{
    public class TestObject
    {
        public TestObject(string name, IList<string> list)
        {
            Name = name;
            List = list;
        }

        public string Name { get; set; }

        public IList<string> List { get; set; }
    }
}