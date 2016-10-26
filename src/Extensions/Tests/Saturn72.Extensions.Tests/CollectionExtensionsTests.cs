#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class CollectionExtensionsTests
    {
        [Test]
        public
        void AddIfNotExists_NotAddingExistsItem()
        {
            var source = new List<int> {1, 2, 3};
            source.AddIfNotExist(3);

            var all = source.Where(x => x == 3);
            Assert.AreEqual(1, all.Count());
        }

        [Test]
        public
        void AddIfNotExists_AddsNewItem()
        {
            var source = new List<int> {1, 2, 3};
            source.AddIfNotExist(4);

            Assert.Contains(4, source);
        }


        [Test]
        public
        void AddIfNotExists_ThrowsOnNullItemToAdd()
        {
            Assert.Throws<NullReferenceException>
                (() => (null as IList<object>)
                .AddIfNotExist(new object()));
        }
    }
}