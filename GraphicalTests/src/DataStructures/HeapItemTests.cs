using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Graphical.DataStructures.Tests
{
    [TestFixture]
    public class HeapItemTest
    {

        [Test]
        public void ComparisonTest()
        {
            var items = TestItems();

            Assert.IsTrue(items[0] < items[1]);
            Assert.IsTrue(items[1] > items[0]);
        }

        [Test]
        public void MinAndMaxTest()
        {
            var items = TestItems();
            Assert.IsTrue(items.Min().Value.CompareTo(1) == 0);
            Assert.IsTrue(items.Max().Value.CompareTo(2) == 0);
        }
        

        public HeapItem[] TestItems()
        {
            var a = new HeapItem("item1", 1);
            var b = new HeapItem("item2", 2);

            return new HeapItem[] { a, b };

        }

    }
}