using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures.Tests
{
    [TestFixture]
    public class MinBinaryHeapTests
    {
        [Test]
        public void MinBinaryHeapConstructorTest()
        {
            var minHeapDefault = new MinBinaryHeap<string, int>();
            var minHeapCustom = new MinBinaryHeap<string, int>(16);
            Assert.NotNull(minHeapDefault);
            Assert.NotNull(minHeapCustom);
            Assert.AreEqual(32, minHeapDefault.Capacity);
            Assert.AreEqual(32, minHeapDefault.Capacity);
        }

        [Test]
        public void AddTest()
        {
            var minHeap = TestMinHeap();
            Assert.AreEqual(10, minHeap.Size);
        }

        [Test]
        public void PeekTest()
        {
            var minHeap = TestMinHeap();
            Assert.AreEqual(0, minHeap.Peek());
            Assert.AreEqual(10, minHeap.Size);
        }

        [Test]
        public void TakeTest()
        {
            var minHeap = TestMinHeap();
            Assert.AreEqual(0, minHeap.Take());
            Assert.AreEqual(9, minHeap.Size);
            Assert.AreEqual(1, minHeap.Peek());
        }

        public MinBinaryHeap<int, int> TestMinHeap()
        {
            var values =  new List<int>() { 3, 7, 10, 1, 4, 4, 0, 20, 15, 12 };
            var minHeap = new MinBinaryHeap<int, int>(values.Count);
            values.ForEach(v => minHeap.Add(v, v));

            return minHeap;
        }

    }

    [TestFixture]
    public class MaxBinaryHeapTests
    {
        [Test]
        public void MaxBinaryHeapConstructorTest()
        {
            var minHeapDefault = new MaxBinaryHeap<string, int>();
            var minHeapCustom = new MaxBinaryHeap<string, int>(16);
            Assert.NotNull(minHeapDefault);
            Assert.NotNull(minHeapCustom);
            Assert.AreEqual(32, minHeapDefault.Capacity);
            Assert.AreEqual(32, minHeapDefault.Capacity);
        }

        [Test]
        public void AddTest()
        {
            var minHeap = TestMaxHeap();
            Assert.AreEqual(10, minHeap.Size);
        }

        [Test]
        public void PeekTest()
        {
            var maxHeap = TestMaxHeap();
            Assert.AreEqual(20, maxHeap.Peek());
            Assert.AreEqual(10, maxHeap.Size);
        }

        [Test]
        public void TakeTest()
        {
            var maxHeap = TestMaxHeap();
            Assert.AreEqual(20, maxHeap.Take());
            Assert.AreEqual(9, maxHeap.Size);
            Assert.AreEqual(15, maxHeap.Peek());
        }

        public MaxBinaryHeap<int, int> TestMaxHeap()
        {
            var values = new List<int>() { 3, 7, 10, 1, 4, 4, 0, 20, 15, 12 };
            var maxHeap = new MaxBinaryHeap<int, int>(values.Count);
            values.ForEach(v => maxHeap.Add(v, v));

            return maxHeap;
        }

    }
}