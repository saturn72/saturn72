
using System.Security.Policy;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Extensions.Data.Tests
{
    public class EnumerableExtensionsTests
    {
        public class MyEntity
        {
            public int Id { get; set; }

            public string Key { get; set; }
            public string Value { get; set; }
            public int ForeignKey { get; set; }
        }
        [Test]
        public void ToDataTable_ReturnsEmptyDataset()
        {
            var entities = new[]
            {
                new MyEntity {Id = 1, Key = "key1", Value = "value1", ForeignKey = 1},
                new MyEntity {Id = 2, Key = "key2", Value = "value2", ForeignKey = 2},
                new MyEntity {Id = 3, Key = "key3", Value = "value3", ForeignKey = 3},
                new MyEntity {Id = 4, Key = "key4", Value = "value4", ForeignKey = 1},
                new MyEntity {Id = 5, Key = "key5", Value = "value5", ForeignKey = 2},
                new MyEntity {Id = 6, Key = "key6", Value = "value6", ForeignKey = 3},
                new MyEntity {Id = 7, Key = "key7", Value = "value7", ForeignKey = 1},
                new MyEntity {Id = 8, Key = "key8", Value = "value8", ForeignKey = 2},
                new MyEntity {Id = 9, Key = "key9", Value = "value9", ForeignKey = 3},
                new MyEntity {Id = 10, Key = "key10", Value = "value10", ForeignKey = 1},
                new MyEntity {Id = 11, Key = "key11", Value = "value11", ForeignKey = 2},
            };

            var result = entities.ToDataTable();
            result.Rows.Count.ShouldEqual(11);
            for (var i = 1; i <= 11; i++)
            {
                var row = result.Rows[i - 1];
                row["Id"].ShouldEqual(i);
                row["Key"].ShouldEqual("key" + i);
                row["Value"].ShouldEqual("value" + i);
                row["ForeignKey"].ShouldEqual(i%3 == 0 ? 3 : i%3);
            }
        }
    }
}
