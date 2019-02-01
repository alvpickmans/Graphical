using NUnit.Framework;
using Graphical.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicalTests.src.Geometry
{
    [TestFixture]
    public class gBoundingBoxTests
    {

        [Test]
        public void IntersectsTest()
        {
            var mainBbox = gBoundingBox.ByMinVertexMaxVertex(
                gVertex.ByCoordinates(-5, -5, -5),
                gVertex.ByCoordinates(5, 5, 5)
                );
            var intersecting = gBoundingBox.ByMinVertexMaxVertex(
                gVertex.ByCoordinates(0, 0, 0),
                gVertex.ByCoordinates(10, 10, 10)
                );
            var coincidentAtVertex = gBoundingBox.ByMinVertexMaxVertex(
                gVertex.ByCoordinates(5, 5, 5),
                gVertex.ByCoordinates(10, 10, 10)
                );
            var coincidentAtEdge = gBoundingBox.ByMinVertexMaxVertex(
                gVertex.ByCoordinates(5, -5, 5),
                gVertex.ByCoordinates(15, 5, 15)
                );
            var interior = gBoundingBox.ByMinVertexMaxVertex(
               gVertex.ByCoordinates(-2.5, -2.5, -2.5),
               gVertex.ByCoordinates(2.5, 2.5, 2.5)
               );
            var noIntersecting = gBoundingBox.ByMinVertexMaxVertex(
                gVertex.ByCoordinates(15, 15, 15),
                gVertex.ByCoordinates(20, 20, 20)
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
            var edge1 = gEdge.ByStartVertexEndVertex(
               gVertex.ByCoordinates(0, 0, 0),
               gVertex.ByCoordinates(5, 0, 5)
               );
            var vertex1 = gVertex.ByCoordinates(5, 0, 2.5);

            var edge2 = gEdge.ByStartVertexEndVertex(
               gVertex.ByCoordinates(7, 1, 0),
               gVertex.ByCoordinates(3, 5, 0)
               );
            var edge3 = gEdge.ByStartVertexEndVertex(
              gVertex.ByCoordinates(9, 8, 0),
              gVertex.ByCoordinates(1, 3, 0)
              );

            //Assert.IsTrue(edge1.BoundingBox.Intersects(vertex1.BoundingBox));
            Assert.IsTrue(edge2.BoundingBox.Intersects(edge3.BoundingBox));
        }

    }
}
