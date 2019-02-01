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
    public class gEdgeTests
    {
        //[Test]
        public void ByStartVertexEndVertexTest()
        {
            
        }

        //[Test]
        public void ContainsTest()
        {
            Assert.Fail();
        }

        //[Test]
        public void GetVertexPairTest()
        {
            Assert.Fail();
        }

        [Test]
        public void IsCoplanarToTest()
        {
            gVertex v1 = gVertex.ByCoordinates(0, 0, 0);
            gVertex v2 = gVertex.ByCoordinates(0, 10, 0);
            gVertex v3 = gVertex.ByCoordinates(10, 10, 10);
            gVertex v4 = gVertex.ByCoordinates(-10, 10, 10);
            var a = gEdge.ByStartVertexEndVertex(v1, v2); // Vertical y axis
            var b = gEdge.ByStartVertexEndVertex(v1, v3); // angled coincident on origin
            var c = gEdge.ByStartVertexEndVertex(v3, v4); // parallel to xy plane

            Assert.IsTrue(a.IsCoplanarTo(b));
            Assert.IsFalse(a.IsCoplanarTo(c));
            Assert.IsTrue(b.IsCoplanarTo(c));

        }

        [Test]
        public void IntersectionTest()
        {
            gVertex a1 = gVertex.ByCoordinates(0, 0, 0);
            gVertex a2 = gVertex.ByCoordinates(10, 10, 10);
            gVertex b1 = gVertex.ByCoordinates(0, 10, 0);
            gVertex b2 = gVertex.ByCoordinates(10, 0, 10);
            gVertex c1 = gVertex.ByCoordinates(0, 10, 5);
            gVertex c2 = gVertex.ByCoordinates(10, 10, 5);
            gVertex d1 = gVertex.ByCoordinates(0, 15, 0);
            gVertex d2 = gVertex.ByCoordinates(10, 15, 10);
            gVertex e1 = gVertex.ByCoordinates(10, 0, 0);
            gVertex e2 = gVertex.ByCoordinates(10, 10, 0);
            gVertex f1 = a1;
            gVertex f2 = b1;
            var a = gEdge.ByStartVertexEndVertex(a1, a2); 
            var b = gEdge.ByStartVertexEndVertex(b1, b2); 
            var c = gEdge.ByStartVertexEndVertex(c1, c2);
            var d = gEdge.ByStartVertexEndVertex(d1, d2);
            var e = gEdge.ByStartVertexEndVertex(e1, e2);
            var f = gEdge.ByStartVertexEndVertex(f1, f2);
            var g = gEdge.ByStartVertexEndVertex(a1, e1);
            var h = gEdge.ByStartVertexEndVertex(b1, gVertex.ByCoordinates(2.5, 5, 0));
            var xaligned = gEdge.ByStartVertexEndVertex(a1, gVertex.ByCoordinates(114.750, 0, 0));
            var xCoincident = gEdge.ByStartVertexEndVertex(e1, b1);

            var side = gEdge.ByStartVertexEndVertex(
                    gVertex.ByCoordinates(-17.950, 31.090, 0) , gVertex.ByCoordinates(-7.650, 48.930,0)
                );
            var rayEdge = gEdge.ByStartVertexEndVertex(
                    gVertex.ByCoordinates(-49.544, 39.031, 0), gVertex.ByCoordinates(5827.960, 39.031, 0)
                );

            var vertical = gEdge.ByStartVertexEndVertex(a1, b1);
            var otherRay = gEdge.ByStartVertexEndVertex(
                    gVertex.ByCoordinates(0, 5, 0), gVertex.ByCoordinates(15, 5, 0)
                );

            var coincidentProjection1 = gEdge.ByStartVertexEndVertex(
                    gVertex.ByCoordinates(35, 30),
                    gVertex.ByCoordinates(13.571429, 17.142857)
                );
            var coincidentProjection2 = gEdge.ByStartVertexEndVertex(
                    gVertex.ByCoordinates(17, 28),
                    gVertex.ByCoordinates(8, 25.5)
                );

            gBase ab = a.Intersection(b); // Intersecting
            gBase ac = a.Intersection(c); // Skew edges
            gBase ad = a.Intersection(d); // Coplanar but not intersecting
            gBase ef = e.Intersection(f); // Coplanar and parallel
            gBase gh = g.Intersection(h); // Coplanar, not intersecting and second edge shorter than first

            //Assert.NotNull(ab);
            //Assert.AreEqual(5, (ab as gVertex).X);
            //Assert.AreEqual(5, (ab as gVertex).Y);
            //Assert.IsNull(ac);
            //Assert.IsNull(ad);
            //Assert.IsNull(ef);
            //Assert.IsNull(gh);
            //Assert.NotNull(rayEdge.Intersection(side));
            //Assert.NotNull(xaligned.Intersection(xCoincident));
            //Assert.AreEqual(otherRay.StartVertex, otherRay.Intersection(vertical));
            Assert.IsNull(coincidentProjection1.Intersection(coincidentProjection2));
        }

        //[Test]
        public void DistanceToTest()
        {
            Assert.Fail();
        }

        //[Test]
        public void DistanceToTest1()
        {
            Assert.Fail();
        }

        //[Test]
        public void EqualsTest()
        {
            Assert.Fail();
        }
    }
}