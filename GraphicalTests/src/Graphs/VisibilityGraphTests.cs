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
    public class VisibilityGraphTests
    {
        [Test]
        public void VertexInPolygonTest()
        {
            var origin = Vertex.ByCoordinates(0, 0, 0);
            var a = Vertex.ByCoordinates(20, 0, 0);
            var b = Vertex.ByCoordinates(0, 20, 0);
            var c = Vertex.ByCoordinates(-20, 0, 0);
            var d = Vertex.ByCoordinates(0, -20, 0);
            var edges = new List<Edge> {
                Edge.ByStartVertexEndVertex(a, b),
                Edge.ByStartVertexEndVertex(b, c),
                Edge.ByStartVertexEndVertex(c, d),
                Edge.ByStartVertexEndVertex(d, a)
            };
            var pol = Polygon.ByVertices(new List<Vertex>() { a, b, c, d }, false);
            

            bool inside = pol.ContainsVertex(origin);

            Assert.IsTrue(inside);
        }

        [Test]
        public void VisibilityFromPointColinearVerticesXAxis()
        {
            var a = Vertex.ByCoordinates(-20, -20);
            var b = Vertex.ByCoordinates(a.X, a.Y - 5);
            var c = Vertex.ByCoordinates(b.X, b.X - 10);
            var d = Vertex.ByCoordinates(c.X + 10, c.Y);
            var e = Vertex.ByCoordinates(d.X, b.Y);
            var f = Vertex.ByCoordinates(e.X + 5, e.Y);
            var g = Vertex.ByCoordinates(f.X, d.Y);
            var h = Vertex.ByCoordinates(g.X + 10, g.Y);
            var i = Vertex.ByCoordinates(h.X, f.Y);
            var j = Vertex.ByCoordinates(i.X, a.Y);

            Polygon polygon = Polygon.ByVertices(new List<Vertex>() { a, b, c, d, e, f, g, h, i, j }, true);
            Graph baseGraph = new Graph(new List<Polygon>() { polygon });
            List<Vertex> vertices = VisibilityGraph.VertexVisibility(i, baseGraph);

            Assert.NotNull(vertices);
        }

        [Test]
        public void VisibilityFromPointColinearVerticesYAxis()
        {
            var a = Vertex.ByCoordinates(0, 0);
            var b = Vertex.ByCoordinates(a.X + 20, a.Y);
            var c = Vertex.ByCoordinates(b.X, b.Y + 10);
            var d = Vertex.ByCoordinates(c.X - 10, c.Y);
            var e = Vertex.ByCoordinates(d.X, d.Y + 10);
            var f = Vertex.ByCoordinates(c.X, e.Y);
            var g = Vertex.ByCoordinates(f.X, d.Y + 10);
            var h = Vertex.ByCoordinates(a.X, g.Y);

            Polygon polygon = Polygon.ByVertices(new List<Vertex>() { a, b, c, d, e, f, g, h}, true);
            Graph baseGraph = new Graph(new List<Polygon>() { polygon });

            List<Vertex> vertices = VisibilityGraph.VertexVisibility(b, baseGraph);

            Assert.NotNull(vertices);
        }

    }
}