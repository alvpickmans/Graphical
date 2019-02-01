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
            var a = gVertex.ByCoordinates(0, 0, 0);
            var b = gVertex.ByCoordinates(0, 10, 0);
            var c = gVertex.ByCoordinates(10, 10, 0);
            var d = gVertex.ByCoordinates(15, 25, 0);
            var e = gVertex.ByCoordinates(0, 50, 3);
            var f = gVertex.ByCoordinates(15, 15, 0.5);
            var g = gVertex.ByCoordinates(10, 0, 0);
            var h = gVertex.ByCoordinates(5, 0, 10);

            gPolygon pol1 = gPolygon.ByVertices(new List<gVertex>() { a, b, c, d });
            gPolygon pol2 = gPolygon.ByVertices(new List<gVertex>() { a, e, f });
            gPolygon triangleXZPlane = gPolygon.ByVertices(new List<gVertex>() { a, g, h });

            Assert.IsTrue(gPolygon.IsPlanar(pol1));
            Assert.IsTrue(gPolygon.IsPlanar(pol2));
            Assert.IsTrue(gPolygon.IsPlanar(triangleXZPlane));
        }

        [Test]
        public void ContainsVertexTest()
        {
            var a = gVertex.ByCoordinates(0, 0);
            var b = gVertex.ByCoordinates(0, 10);
            var c = gVertex.ByCoordinates(5, 15);
            var d = gVertex.ByCoordinates(10, 10);
            var e = gVertex.ByCoordinates(10, 0);
            var vtx1 = gVertex.ByCoordinates(0, 5);
            var vtx2 = gVertex.ByCoordinates(5, 5);
            var vtx3 = gVertex.ByCoordinates(10, 5);
            var vtx4 = gVertex.ByCoordinates(0, 15);
            gPolygon pol1 = gPolygon.ByVertices(new List<gVertex>() { a, b, c, d, e });

            Assert.IsTrue(pol1.ContainsVertex(vtx1));
            Assert.IsTrue(pol1.ContainsVertex(vtx2));
            Assert.IsTrue(pol1.ContainsVertex(vtx3));
            Assert.IsFalse(pol1.ContainsVertex(vtx4));
        }

        [Test]
        public void ContainsEdgeTest()
        {
            var a = gVertex.ByCoordinates(0, 0);
            var b = gVertex.ByCoordinates(0, 10);
            var c = gVertex.ByCoordinates(10, 10);
            var d = gVertex.ByCoordinates(10, 0);
            var vtx1 = gVertex.ByCoordinates(0, 5);
            var vtx2 = gVertex.ByCoordinates(5, 5);
            var vtx3 = gVertex.ByCoordinates(10, 5);
            var vtx4 = gVertex.ByCoordinates(5, 15);
            gPolygon pol1 = gPolygon.ByVertices(new List<gVertex>() { a, b, c, d });

            Assert.IsTrue(pol1.ContainsEdge(gEdge.ByStartVertexEndVertex(vtx1, vtx2)));
            Assert.IsTrue(pol1.ContainsEdge(gEdge.ByStartVertexEndVertex(vtx1, vtx3)));
            Assert.IsFalse(pol1.ContainsEdge(gEdge.ByStartVertexEndVertex(vtx1, vtx4)));

        }

        [Test]
        public void RegularPolygon()
        {
            var square = gPolygon.ByCenterRadiusAndSides(gVertex.Origin(), 10, 4);

            Assert.AreEqual(gVertex.ByCoordinates(0, 10, 0), square.Vertices[0]);
            Assert.AreEqual(gVertex.ByCoordinates(10, 0, 0), square.Vertices[1]);
        }
    }
}