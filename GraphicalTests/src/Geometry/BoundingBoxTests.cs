using NUnit.Framework;
using Graphical.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Geometry.Tests
{
    [TestFixture]
    public class BoundingBoxTests
    {

        [Test]
        public void IntersectsTest()
        {
            var mainBbox = BoundingBox.ByMinVertexMaxVertex(
                Vertex.ByCoordinates(-5, -5, -5),
                Vertex.ByCoordinates(5, 5, 5)
                );
            var intersecting = BoundingBox.ByMinVertexMaxVertex(
                Vertex.ByCoordinates(0, 0, 0),
                Vertex.ByCoordinates(10, 10, 10)
                );
            var coincidentAtVertex = BoundingBox.ByMinVertexMaxVertex(
                Vertex.ByCoordinates(5, 5, 5),
                Vertex.ByCoordinates(10, 10, 10)
                );
            var coincidentAtEdge = BoundingBox.ByMinVertexMaxVertex(
                Vertex.ByCoordinates(5, -5, 5),
                Vertex.ByCoordinates(15, 5, 15)
                );
            var interior = BoundingBox.ByMinVertexMaxVertex(
               Vertex.ByCoordinates(-2.5, -2.5, -2.5),
               Vertex.ByCoordinates(2.5, 2.5, 2.5)
               );
            var noIntersecting = BoundingBox.ByMinVertexMaxVertex(
                Vertex.ByCoordinates(15, 15, 15),
                Vertex.ByCoordinates(20, 20, 20)
                );

            Assert.IsTrue(mainBbox.Intersects(intersecting));
            Assert.IsTrue(mainBbox.Intersects(coincidentAtVertex));
            Assert.IsTrue(mainBbox.Intersects(coincidentAtEdge));
            Assert.IsTrue(mainBbox.Intersects(interior));
            Assert.IsFalse(mainBbox.Intersects(noIntersecting));
        }

        [Test]
        public void IntersectsGeometrisBboxTests()
        {
            var edge1 = Edge.ByStartVertexEndVertex(
               Vertex.ByCoordinates(0, 0, 0),
               Vertex.ByCoordinates(5, 0, 5)
               );
            var vertex1 = Vertex.ByCoordinates(5, 0, 2.5);

            var edge2 = Edge.ByStartVertexEndVertex(
               Vertex.ByCoordinates(7, 1, 0),
               Vertex.ByCoordinates(3, 5, 0)
               );
            var edge3 = Edge.ByStartVertexEndVertex(
              Vertex.ByCoordinates(9, 8, 0),
              Vertex.ByCoordinates(1, 3, 0)
              );

            //Assert.IsTrue(edge1.BoundingBox.Intersects(vertex1.BoundingBox));
            Assert.IsTrue(edge2.BoundingBox.Intersects(edge3.BoundingBox));
        }

    }
}
