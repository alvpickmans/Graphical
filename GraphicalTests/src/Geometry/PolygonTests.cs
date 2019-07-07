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
            Polygon pol1 = Polygon.ByVertices(new List<Vertex>() { a, b, c, d, e });

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
        public void Intersection_Pass_EdgeThroughMiddle()
        {
            Vertex[] vertices = new Vertex[4]
            {
                Vertex.ByCoordinates(0,0,0),
                Vertex.ByCoordinates(20,0,0),
                Vertex.ByCoordinates(20,20,0),
                Vertex.ByCoordinates(0,20,0),
            };

            Polygon polygon = Polygon.ByVertices(vertices.ToList());
            Edge edge = Edge.ByCoordinatesArray(new double[6] { -5, 5, 0, 25, 5, 0 });

            List<Geometry> intersection = polygon.Intersection(edge);

            var intersection1 = Vertex.ByCoordinates(0, 5, 0);
            var intersection2 = Vertex.ByCoordinates(20, 5, 0);

            Assert.AreEqual(2, intersection.Count);
            CollectionAssert.Contains(intersection, intersection1);
            CollectionAssert.Contains(intersection, intersection2);
        }

        [Test]
        public void Intersection_Pass_NoIntersectionEvenWhenBboxIntersects()
        {
            Vertex[] vertices = new Vertex[4]
            {
                Vertex.ByCoordinates(0,0,0),
                Vertex.ByCoordinates(20,0,0),
                Vertex.ByCoordinates(20,20,0),
                Vertex.ByCoordinates(0,20,0),
            };

            Polygon polygon = Polygon.ByVertices(vertices.ToList());
            Edge edge = Edge.ByCoordinatesArray(new double[6] { -5, 15, 0, 5, 30, 0 });

            List<Geometry> intersection = polygon.Intersection(edge);

            CollectionAssert.IsEmpty(intersection);

        }

        [Test]
        public void Intersection_Pass_WhenEdgeIntersectsFirstDiagonal([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetIrregularPentagon(ccw);
            Edge edge = Edge.ByCoordinatesArray(new double[6] { 18, -5, 0, 10, 25, 0 });
            Edge edgeInverse = Edge.ByStartVertexEndVertex(edge.EndVertex, edge.StartVertex);

            var intersection = polygon.Intersection(edge);
            var intersectionInverse = polygon.Intersection(edgeInverse);

            CollectionAssert.AllItemsAreInstancesOfType(intersection, typeof(Vertex));
            CollectionAssert.AllItemsAreInstancesOfType(intersectionInverse, typeof(Vertex));

            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(2, intersectionInverse.Count);
        }

        [Test]
        public void Intersection_Pass_WhenEdgeIsOnRight([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetIrregularPentagon(ccw);
            Edge edge = Edge.ByCoordinatesArray(new double[6] { 15, 0, 0, 25, 10, 0 });
            Edge edgeInverse = Edge.ByStartVertexEndVertex(edge.EndVertex, edge.StartVertex);

            var intersection = polygon.Intersection(edge);
            var intersectionInverse = polygon.Intersection(edgeInverse);

            CollectionAssert.AllItemsAreInstancesOfType(intersection, typeof(Vertex));
            CollectionAssert.AllItemsAreInstancesOfType(intersectionInverse, typeof(Vertex));
            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(2, intersectionInverse.Count);
        }

        [Test]
        public void Intersection_Pass_WhenEdgeIsOnLeft([Values(true, false)] bool ccw)
        {
            Polygon polygon = GetIrregularPentagon(ccw);
            Edge edge = Edge.ByCoordinatesArray(new double[6] { 5, 0, 0, 10, 30, 0 });
            Edge edgeInverse = Edge.ByStartVertexEndVertex(edge.EndVertex, edge.StartVertex);

            var intersection = polygon.Intersection(edge);
            var intersectionInverse = polygon.Intersection(edgeInverse);

            CollectionAssert.AllItemsAreInstancesOfType(intersection, typeof(Vertex));
            CollectionAssert.AllItemsAreInstancesOfType(intersectionInverse, typeof(Vertex));
            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(2, intersectionInverse.Count);
        }

        [Test]
        public void IntersectionTest()
        {
            var square = Polygon.ByCenterRadiusAndSides(Vertex.Origin(), 5, 7);

            var edge1 = Edge.ByCoordinatesArray(new double[] { -8.25, -5, 0, 6, -2, 0 });

            var intersections1 = square.Intersection(edge1);

            Assert.AreEqual(2, intersections1.Count);

            var polygon = Polygon.ByVertices(new List<Vertex>()
            {
                Vertex.ByCoordinates(26.671, 20.669),
                Vertex.ByCoordinates(24.836, 21.161),
                Vertex.ByCoordinates(23.492, 19.818),
                Vertex.ByCoordinates(23.984, 17.982),
                Vertex.ByCoordinates(25.819, 17.491),
                Vertex.ByCoordinates(27.163, 18.834)
            });

            var edge = Edge.ByCoordinatesArray(new double[] { 13.104, 5.930, 0, 28.756, 27.385, 0 });

            var intersections = polygon.Intersection(edge);

            Assert.IsEmpty(intersections);

            var polygon2 = Polygon.ByVertices(new List<Vertex>()
            {
                Vertex.ByCoordinates(5, -2),
                Vertex.ByCoordinates(10, -4),
                Vertex.ByCoordinates(15, -2),
                Vertex.ByCoordinates(10, 10)
            });

            var edge2 = Edge.ByCoordinatesArray(new double[] { 0, 2, 0, 20, 2, 0 });

            Assert.IsNotEmpty(polygon2.Intersection(edge2));
        }

        [Test]
        public void BelongsTest()
        {
            var a = Vertex.ByCoordinates(0, 0);
            var b = Vertex.ByCoordinates(10, 0);
            var c = Vertex.ByCoordinates(8, 5);
            var d = Vertex.ByCoordinates(10, 10);
            var e = Vertex.ByCoordinates(0, 10);

            var polygon = Polygon.ByVertices(new List<Vertex>() { a, b, c, d, e });

            var polygonEdge = Edge.ByStartVertexEndVertex(a, e);
            var edgeFromPolygonVertices = Edge.ByStartVertexEndVertex(d, b);
            var externalEdge = Edge.ByCoordinatesArray(new double[] { 0, 15, 0, 10, 20, 0 });

            Assert.IsTrue(polygon.Belongs(polygonEdge));
            Assert.IsFalse(polygon.Belongs(edgeFromPolygonVertices));
            Assert.IsFalse(polygon.Belongs(externalEdge));
        }

        private Polygon GetIrregularPentagon(bool counterClockwise = true, bool randomShift = false)
        {
            Vertex[] vertices = new Vertex[5]
            {
                Vertex.ByCoordinates(10,0,0),
                Vertex.ByCoordinates(20,0,0),
                Vertex.ByCoordinates(25,20,0),
                Vertex.ByCoordinates(15,30,0),
                Vertex.ByCoordinates(5,20,0),
            };

            if (randomShift)
            {
                int shift = new Random().Next(1, vertices.Length);

                Vertex[] temp = new Vertex[vertices.Length];

                for (int i = 0; i < vertices.Length; i++)
                {
                    int index = (shift + i) % vertices.Length;
                    temp[i] = vertices[index];
                }

                vertices = temp;
            }

            if (counterClockwise)
                return Polygon.ByVertices(vertices.ToList());
            else
                return Polygon.ByVertices(vertices.Reverse().ToList());
        }
    }
}