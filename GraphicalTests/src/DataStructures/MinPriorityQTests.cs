using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures.Tests
{
    [TestFixture]
    public class MinPriorityQTests
    {
        [Test]
        public void MinPriorityQTest()
        {
            var minQueueDefault = new MinPriorityQ<int, int>();
            var minQueueCustom = new MinPriorityQ<int, int>(16);
            Assert.AreEqual(32, minQueueDefault.Capacity);
            Assert.AreEqual(16, minQueueCustom.Capacity);
        }

        [Test]
        public void AddTest()
        {
            var minQ = TestMinQ();
            Assert.AreEqual(10, minQ.Size);
            Assert.AreEqual(10, minQ.HeapIndices.Count);
        }

        [Test]
        public void TakeTest()
        {
            var minQ = TestMinQ();
            Assert.AreEqual(10, minQ.Size);
            Assert.AreEqual(10, minQ.HeapIndices.Count);
            Assert.AreEqual(0, minQ.Take());
            Assert.AreEqual(1, minQ.Take());
            Assert.AreEqual(8, minQ.Size);
            Assert.AreEqual(8, minQ.HeapIndices.Count);
        }

        [Test]
        public void UpdateTest()
        {
            var minQ = new MinPriorityQ<string, int>();
            minQ.Add("seis", 6);
            minQ.Add("diez", 10);
            minQ.Add("dos", 2);
            minQ.Add("uno", 1);
            Assert.AreEqual(4, minQ.Size);
            Assert.AreEqual("uno", minQ.Peek());
            minQ.UpdateValue("diez", 0);
            Assert.AreEqual("diez", minQ.Peek());

        }

        public MinPriorityQ<int, int> TestMinQ()
        {
            var values = new List<int>() { 3, 7, 10, 1, 4, 6, 0, 20, 15, 12 };
            var minQ = new MinPriorityQ<int, int>(values.Count);
            values.ForEach(v => minQ.Add(v, v));

            return minQ;
        }
    }
}