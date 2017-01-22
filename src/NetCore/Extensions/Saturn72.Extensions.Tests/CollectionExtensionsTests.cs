#region

using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.UnitTesting.Framework;
using Xunit;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public
        void AddIfNotExists_NotAddingExistsItem()
        {
            var source = new List<int> {1, 2, 3};
            source.AddIfNotExist(3);

            var all = source.Where(x => x == 3);
            1.ShouldEqual(all.Count());
        }

        [Fact]
        public
        void AddIfNotExists_AddsNewItem()
        {
            var source = new List<int> {1, 2, 3};
            source.AddIfNotExist(4);

            Assert.Contains(4, source);
        }


        [Fact]
        public
        void AddIfNotExists_ThrowsOnNullItemToAdd()
        {
            Assert.Throws<NullReferenceException>
                (() => (null as IList<object>)
                .AddIfNotExist(new object()));
        }
    }
}