using Graphical.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Graphical.Geometry.Tests
{
    [TestFixture]
    public class gPolygonTests
    {
        [Test]
        public void IsPlanarTest()
        {
            var a = Vertex.ByCoordinates(0, 0, 0);
            var b = Vertex.ByCoordinates(0, 10, 0);
            var c = Vertex.ByCoordinates(10, 10, 0);
            var d = Vertex.ByCoordinates(15, 25, 0);
            var e = Vertex.ByCoordinates(0, 50, 3);
            var f = Vertex.ByCoordinates(15, 15, 0.5);
            var g = Vertex.ByCoordinates(10, 0, 0);
            var h = Vertex.ByCoordinates(5, 0, 10);

            gPolygon pol1 = gPolygon.ByVertices(new List<Vertex>() { a, b, c, d });
            gPolygon pol2 = gPolygon.ByVertices(new List<Vertex>() { a, e, f });
            gPolygon triangleXZPlane = gPolygon.ByVertices(new List<Vertex>() { a, g, h });

            Assert.IsTrue(gPolygon.IsPlanar(pol1));
            Assert.IsTrue(gPolygon.IsPlanar(pol2));
            Assert.IsTrue(gPolygon.IsPlanar(triangleXZPlane));
        }

        [Test]
        public void ContainsVertexTest()
        {
            var a = Vertex.ByCoordinates(0, 0);
            var b = Vertex.ByCoordinates(0, 10);
            var c = Vertex.ByCoordinates(5, 15);
            var d = Vertex.ByCoordinates(10, 10);
            var e = Vertex.ByCoordinates(10, 0);
            var vtx1 = Vertex.ByCoordinates(0, 5);
            var vtx2 = Vertex.ByCoordinates(5, 5);
            var vtx3 = Vertex.ByCoordinates(10, 5);
            var vtx4 = Vertex.ByCoordinates(0, 15);
            gPolygon pol1 = gPolygon.ByVertices(new List<Vertex>() { a, b, c, d, e });

            Assert.IsTrue(pol1.ContainsVertex(vtx1));
            Assert.IsTrue(pol1.ContainsVertex(vtx2));
            Assert.IsTrue(pol1.ContainsVertex(vtx3));
            Assert.IsFalse(pol1.ContainsVertex(vtx4));
        }

        [Test]
        public void ContainsEdgeTest()
        {
            var a = Vertex.ByCoordinates(0, 0);
            var b = Vertex.ByCoordinates(0, 10);
            var c = Vertex.ByCoordinates(10, 10);
            var d = Vertex.ByCoordinates(10, 0);
            var vtx1 = Vertex.ByCoordinates(0, 5);
            var vtx2 = Vertex.ByCoordinates(5, 5);
            var vtx3 = Vertex.ByCoordinates(10, 5);
            var vtx4 = Vertex.ByCoordinates(5, 15);
            gPolygon pol1 = gPolygon.ByVertices(new List<Vertex>() { a, b, c, d });

            Assert.IsTrue(pol1.ContainsEdge(gEdge.ByStartVertexEndVertex(vtx1, vtx2)));
            Assert.IsTrue(pol1.ContainsEdge(gEdge.ByStartVertexEndVertex(vtx1, vtx3)));
            Assert.IsFalse(pol1.ContainsEdge(gEdge.ByStartVertexEndVertex(vtx1, vtx4)));

        }

        [Test]
        public void RegularPolygon()
        {
            var square = gPolygon.ByCenterRadiusAndSides(Vertex.Origin(), 10, 4);

            Assert.AreEqual(Vertex.ByCoordinates(0, 10, 0), square.Vertices[0]);
            Assert.AreEqual(Vertex.ByCoordinates(10, 0, 0), square.Vertices[1]);
        }
    }
}