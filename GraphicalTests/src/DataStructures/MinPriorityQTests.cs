using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Core;
using Graphical.Geometry;

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
            //Assert.AreEqual(10, minQ.Size);
            //Assert.AreEqual(10, minQ.HeapIndices.Count);

            var a1 = gVertex.ByCoordinates(0, 0);
            var a2 = gVertex.ByCoordinates(10, 10);
            var b1 = gVertex.ByCoordinates(0, 10);
            var b2 = gVertex.ByCoordinates(10, 0);

            List<gEdge> edges = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(b1,b2)
            };

            var EventsQ = new MinPriorityQ<SweepEvent>(edges.Count * 2);

            foreach (gEdge e in edges)
            {
                var sw1 = new SweepEvent(e.StartVertex, e);
                var sw2 = new SweepEvent(e.EndVertex, e);
                sw1.Pair = sw2;
                sw2.Pair = sw1;
                sw1.IsLeft = sw1 < sw1.Pair;
                sw2.IsLeft = !sw1.IsLeft;
                EventsQ.Add(sw2);
                EventsQ.Add(sw1);
            }

            Assert.AreEqual(a1, EventsQ.Take().Vertex);
            Assert.AreEqual(b1, EventsQ.Take().Vertex);
            Assert.AreEqual(b2, EventsQ.Take().Vertex);
            Assert.AreEqual(a2, EventsQ.Take().Vertex);
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
            minQ.UpdateItem("diez", 0);
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