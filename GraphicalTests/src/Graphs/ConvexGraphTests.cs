using Graphical.Graphs;
using Graphical.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Graphical.Graphs.Tests
{
    [TestFixture]
    public class ConvexGraphTests
    {
        [Test]
        public void ConstructorTest()
        {
            var a1 = Vertex.ByCoordinates(5, -2);
            var b1 = Vertex.ByCoordinates(7, -4);
            var c1 = Vertex.ByCoordinates(10, -2);
            var d1 = Vertex.ByCoordinates(7, 10);

            var polygon1 = Polygon.ByVertices(new List<Vertex>() { a1, b1, c1, d1 });

            var a2 = Vertex.ByCoordinates(10, 10);
            var b2 = Vertex.ByCoordinates(12, 8);
            var c2 = Vertex.ByCoordinates(15, 10);
            var d2 = Vertex.ByCoordinates(12, -2);

            var polygon2 = Polygon.ByVertices(new List<Vertex>() { a2, b2, c2, d2 });

            Graph basegraph = new Graph(new List<Polygon>()
            {
                polygon1, polygon2
            });

            var origin = Vertex.ByCoordinates(8.5, 18.769);
            var destination = Vertex.ByCoordinates(20, 2);

            ConvexGraph convexGraph = ConvexGraph.ByGraphOriginAndDestination(basegraph, origin, destination);

            Assert.IsTrue(convexGraph.Edges.Any());


        }

    }
}