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
    public class PolygonTests
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

            Polygon pol1 = Polygon.ByVertices(new List<Vertex>() { a, b, c, d });
            Polygon pol2 = Polygon.ByVertices(new List<Vertex>() { a, e, f });
            Polygon triangleXZPlane = Polygon.ByVertices(new List<Vertex>() { a, g, h });

            Assert.IsTrue(pol1.IsPlanar());
            Assert.IsTrue(pol2.IsPlanar());
            Assert.IsTrue(triangleXZPlane.IsPlanar());
        }

        [Test]
        public void ContainsVertex_Pass_WhenInsidePolygon([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetPolygon(ccw);
            Vertex vertex = Vertex.ByCoordinates(0, 5);

            Assert.IsTrue(polygon.ContainsVertex(vertex));
        }

        [Test]
        public void ContainsVertex_Pass_WhenIsPolygonVertex([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetPolygon(ccw);
            Vertex vertex1 = Vertex.ByCoordinates(10, 0);
            Vertex vertex2 = Vertex.ByCoordinates(0, 10);

            Assert.IsTrue(polygon.ContainsVertex(vertex1));
            Assert.IsTrue(polygon.ContainsVertex(vertex2));
        }

        [Test]
        public void ContainsVertex_Pass_WhenIsOutside([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetPolygon(ccw);
            Vertex vertex1 = Vertex.ByCoordinates(-5, 5);
            Vertex vertex2 = Vertex.ByCoordinates(0, 20);

            Assert.IsFalse(polygon.ContainsVertex(vertex1));
            Assert.IsFalse(polygon.ContainsVertex(vertex2));
        }

        [Test]
        public void ContainsVertex_Pass_WhenNotInsideButAlignedWithPolygonVertex([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetPolygon(ccw);
            Vertex vertex = Vertex.ByCoordinates(0, 15);

            Assert.IsFalse(polygon.ContainsVertex(vertex));
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
            Polygon pol1 = Polygon.ByVertices(new List<Vertex>() { a, b, c, d });

            Assert.IsTrue(pol1.ContainsEdge(Edge.ByStartVertexEndVertex(vtx1, vtx2)));
            Assert.IsTrue(pol1.ContainsEdge(Edge.ByStartVertexEndVertex(vtx1, vtx3)));
            Assert.IsFalse(pol1.ContainsEdge(Edge.ByStartVertexEndVertex(vtx1, vtx4)));

        }

        [Test]
        public void RegularPolygon()
        {
            var square = Polygon.ByCenterRadiusAndSides(Vertex.Origin(), 10, 4);
            var vertex1 = Vertex.ByCoordinates(10, 0, 0);
            var vertex2 = Vertex.ByCoordinates(0, 10, 0);

            Assert.IsTrue(vertex1.Equals(square.Vertices[0]));
            Assert.IsTrue(vertex2.Equals(square.Vertices[1]));
        }

        [Test]
        public void IntersectionTest()
        {
            var square = Polygon.ByCenterRadiusAndSides(Vertex.Origin(), 5, 7);

            var edge1 = Edge.ByCoordinatesArray(new double[] { -8.25, -5, 0, 6, -2, 0 });
            var edge2 = Edge.ByCoordinatesArray(new double[] { -5, 5, 0, 5, 5, 0 });

            var intersections1 = square.Intersection(edge1);

            Assert.AreEqual(2, intersections1.Count);
        }

        private Polygon GetPolygon(bool counterClockwise = true)
        {
            IEnumerable<Vertex> vertices = new Vertex[5]
            {
                Vertex.ByCoordinates(0, 0),
                Vertex.ByCoordinates(10, 0),
                Vertex.ByCoordinates(10, 10),
                Vertex.ByCoordinates(5, 15),
                Vertex.ByCoordinates(0, 10)
            };

            if (!counterClockwise)
                vertices = vertices.Reverse();

            return Polygon.ByVertices(vertices.ToList());
        }

    }
}