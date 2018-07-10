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
            var pol = gPolygon.ByVertices(new List<gVertex>() { a, b, c, d }, false);
            

            bool inside = pol.ContainsVertex(origin);

            Assert.IsTrue(inside);
        }

        [Test]
        public void VisibilityFromPointColinearVerticesXAxis()
        {
            var a = gVertex.ByCoordinates(-20, -20);
            var b = gVertex.ByCoordinates(a.X, a.Y - 5);
            var c = gVertex.ByCoordinates(b.X, b.X - 10);
            var d = gVertex.ByCoordinates(c.X + 10, c.Y);
            var e = gVertex.ByCoordinates(d.X, b.Y);
            var f = gVertex.ByCoordinates(e.X + 5, e.Y);
            var g = gVertex.ByCoordinates(f.X, d.Y);
            var h = gVertex.ByCoordinates(g.X + 10, g.Y);
            var i = gVertex.ByCoordinates(h.X, f.Y);
            var j = gVertex.ByCoordinates(i.X, a.Y);

            gPolygon polygon = gPolygon.ByVertices(new List<gVertex>() { a, b, c, d, e, f, g, h, i, j }, true);
            Graph baseGraph = new Graph(new List<gPolygon>() { polygon });
            List<gVertex> vertices = VisibilityGraph.VertexVisibility(i, baseGraph);

            Assert.NotNull(vertices);
        }

        [Test]
        public void VisibilityFromPointColinearVerticesYAxis()
        {
            var a = gVertex.ByCoordinates(0, 0);
            var b = gVertex.ByCoordinates(a.X + 20, a.Y);
            var c = gVertex.ByCoordinates(b.X, b.Y + 10);
            var d = gVertex.ByCoordinates(c.X - 10, c.Y);
            var e = gVertex.ByCoordinates(d.X, d.Y + 10);
            var f = gVertex.ByCoordinates(c.X, e.Y);
            var g = gVertex.ByCoordinates(f.X, d.Y + 10);
            var h = gVertex.ByCoordinates(a.X, g.Y);

            gPolygon polygon = gPolygon.ByVertices(new List<gVertex>() { a, b, c, d, e, f, g, h}, true);
            Graph baseGraph = new Graph(new List<gPolygon>() { polygon });

            List<gVertex> vertices = VisibilityGraph.VertexVisibility(b, baseGraph);

            Assert.NotNull(vertices);
        }

    }
}