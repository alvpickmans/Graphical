using Graphical.Graphs;
using Graphical.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestServices;

namespace Graphical.Graphs.Tests
{
    [TestFixture]
    public class VisibilityGraphTests
    {
        [Test]
        public void VertexInPolygonTest()
        {
            var origin = gVertex.ByCoordinates(0, 0, 0);
            var a = gVertex.ByCoordinates(20, 0, 0);
            var b = gVertex.ByCoordinates(0, 20, 0);
            var c = gVertex.ByCoordinates(-20, 0, 0);
            var d = gVertex.ByCoordinates(0, -20, 0);
            var edges = new List<gEdge> {
                gEdge.ByStartVertexEndVertex(a, b),
                gEdge.ByStartVertexEndVertex(b, c),
                gEdge.ByStartVertexEndVertex(c, d),
                gEdge.ByStartVertexEndVertex(d, a)
            };

            bool inside = VisibilityGraph.VertexInPolygon(origin, edges, 120);

            Assert.IsTrue(inside);
        }
    }
}