using Graphical.Core;
using Graphical.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Graphical.Core.Tests
{
    [TestFixture]
    public class SweepLineTests
    {
        //[Test]
        public void ByEdgesTest()
        {

        }

        //[Test]
        public void ByPolygonsTest()
        {

        }

        [Test]
        public void FindIntersectionsTest()
        {
            // Similar example as in http://www.webcitation.org/6ahkPQIsN
            gVertex a1 = gVertex.ByCoordinates(1, 3);
            gVertex a2 = gVertex.ByCoordinates(9, 8);
            gVertex b1 = gVertex.ByCoordinates(5, 1);
            gVertex b2 = gVertex.ByCoordinates(9, 4);
            gVertex c1 = gVertex.ByCoordinates(3, 5);
            gVertex c2 = gVertex.ByCoordinates(7, 1);
            gVertex d1 = gVertex.ByCoordinates(3, 9);
            gVertex d2 = gVertex.ByCoordinates(10, 2);

            List<gEdge> edges = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(b1, b2),
                gEdge.ByStartVertexEndVertex(c1, c2),
                gEdge.ByStartVertexEndVertex(d1, d2)
            };

            SweepLine swLine = SweepLine.ByEdges(edges);
            Assert.AreEqual(true, swLine.HasIntersection());
            Assert.AreEqual(4, swLine.GetIntersections().Count);
        }

        [Test]
        public void FindIntersectionsNotHorizontalTest()
        {
            gVertex a1 = gVertex.ByCoordinates(0, 0, 0);
            gVertex a2 = gVertex.ByCoordinates(10, 10, 10);
            gVertex b1 = gVertex.ByCoordinates(0, 10, 0);
            gVertex b2 = gVertex.ByCoordinates(10, 0, 10);
            //gVertex c1 = gVertex.ByCoordinates(0, 0, 0);
            //gVertex c2 = gVertex.ByCoordinates(0, 10, 10);
            //gVertex d1 = gVertex.ByCoordinates(0, 10, 0);
            //gVertex d2 = gVertex.ByCoordinates(0, 0, 10);

            List<gEdge> edges = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(b1, b2)
                // Error when trying non coplanar lines and intersecting on extremes.
                //gEdge.ByStartVertexEndVertex(c1, c2),
                //gEdge.ByStartVertexEndVertex(d1, d2)
            };

            SweepLine swLine = SweepLine.ByEdges(edges);
            Assert.AreEqual(true, swLine.HasIntersection());
            Assert.AreEqual(1, swLine.GetIntersections().Count);
            //Assert.AreEqual(2, swLine.Intersections.Count);
        }

        [Test]
        public void IntersectionCoincidentLinesTest()
        {
            gVertex a1 = gVertex.ByCoordinates(0, 0);
            gVertex a2 = gVertex.ByCoordinates(10, 10);
            gVertex b1 = gVertex.ByCoordinates(5, 5);
            gVertex b2 = gVertex.ByCoordinates(15, 15);
            gVertex c1 = gVertex.ByCoordinates(3, 5);
            gVertex c2 = gVertex.ByCoordinates(7, 1);
            gVertex d1 = gVertex.ByCoordinates(3, 9);
            gVertex d2 = gVertex.ByCoordinates(10, 2);

            List<gEdge> sameEdges = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(a2, a1)
            };
            SweepLine slSameEdges = SweepLine.ByEdges(sameEdges);
            Assert.AreEqual(1, slSameEdges.GetIntersections().Count);

            List<gEdge> sameStart = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(b1, a1)
            };
            SweepLine slSameStart = SweepLine.ByEdges(sameStart);
            Assert.AreEqual(1, slSameStart.GetIntersections().Count);

            List<gEdge> sameEnd = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(a2, b1)
            };
            SweepLine slSameEnd = SweepLine.ByEdges(sameEnd);
            Assert.AreEqual(1, slSameEnd.GetIntersections().Count);

            List<gEdge> overlapping = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, a2),
                gEdge.ByStartVertexEndVertex(b2, b1)
            };
            SweepLine slOverlapping = SweepLine.ByEdges(overlapping);
            Assert.AreEqual(1, slOverlapping.GetIntersections().Count);

            List<gEdge> containing = new List<gEdge>()
            {
                gEdge.ByStartVertexEndVertex(a1, b2),
                gEdge.ByStartVertexEndVertex(a2, b1)
            };
            SweepLine slContaining = SweepLine.ByEdges(containing);
            Assert.AreEqual(1, slContaining.GetIntersections().Count);
        }

        [Test]
        public void ComplexPolygonBoolean()
        {
            var clip = gPolygon.ByVertices(new List<gVertex>()
            {
                gVertex.ByCoordinates(5, 25),
                gVertex.ByCoordinates(5, 30),
                gVertex.ByCoordinates(15, 15),
                gVertex.ByCoordinates(30, 15),
                gVertex.ByCoordinates(20, 10),
                gVertex.ByCoordinates(17, 27)
            });
            var subject = gPolygon.ByVertices(new List<gVertex>()
            {
                gVertex.ByCoordinates(5, 12),
                gVertex.ByCoordinates(35, 30),
                gVertex.ByCoordinates(32, 5),
                gVertex.ByCoordinates(15, 0)
            });
            
            Assert.AreEqual(2, subject.Union(clip).Count);
            Assert.AreEqual(2, subject.Difference(clip).Count);
            Assert.AreEqual(2, subject.Intersection(clip).Count);
        }

        //[Test]
        public void NoIntersectingPolygonBooleanTest()
        {

        }

        [Test]
        public void NotIntersectingPolygons()
        {
            var clip = gPolygon.ByVertices(new List<gVertex>()
            {
                gVertex.ByCoordinates(0, 0),
                gVertex.ByCoordinates(0, 10),
                gVertex.ByCoordinates(15, 5),
                gVertex.ByCoordinates(15, 0)
            });
            var subject = gPolygon.ByVertices(new List<gVertex>()
            {
                gVertex.ByCoordinates(0, 12.5),
                gVertex.ByCoordinates(0, 20),
                gVertex.ByCoordinates(7.5, 20),
                gVertex.ByCoordinates(15, 20),
                gVertex.ByCoordinates(15, 7.5)
            });

            var swLine = SweepLine.BySubjectClipPolygons(subject, clip);
            Assert.IsFalse(swLine.HasIntersection());
            Assert.AreEqual(2, subject.Union(clip).Count);

            var intersection = subject.Intersection(clip);
            Assert.AreEqual(1, intersection.Count);
            Assert.AreEqual(5, intersection[0].Vertices.Count);

            var difference = subject.Difference(clip);
            Assert.AreEqual(1, difference.Count);
            Assert.AreEqual(5, difference[0].Vertices.Count);
        }
    }


}