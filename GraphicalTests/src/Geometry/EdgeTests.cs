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
    public class EdgeTests
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
            Vertex v1 = Vertex.ByCoordinates(0, 0, 0);
            Vertex v2 = Vertex.ByCoordinates(0, 10, 0);
            Vertex v3 = Vertex.ByCoordinates(10, 10, 10);
            Vertex v4 = Vertex.ByCoordinates(-10, 10, 10);
            var a = Edge.ByStartVertexEndVertex(v1, v2); // Vertical y axis
            var b = Edge.ByStartVertexEndVertex(v1, v3); // angled coincident on origin
            var c = Edge.ByStartVertexEndVertex(v3, v4); // parallel to xy plane

            Assert.IsTrue(a.IsCoplanarTo(b));
            Assert.IsFalse(a.IsCoplanarTo(c));
            Assert.IsTrue(b.IsCoplanarTo(c));

        }

        [Test]
        public void IntersectionTest()
        {
            Vertex a1 = Vertex.ByCoordinates(0, 0, 0);
            Vertex a2 = Vertex.ByCoordinates(10, 10, 10);
            Vertex b1 = Vertex.ByCoordinates(0, 10, 0);
            Vertex b2 = Vertex.ByCoordinates(10, 0, 10);
            Vertex c1 = Vertex.ByCoordinates(0, 10, 5);
            Vertex c2 = Vertex.ByCoordinates(10, 10, 5);
            Vertex d1 = Vertex.ByCoordinates(0, 15, 0);
            Vertex d2 = Vertex.ByCoordinates(10, 15, 10);
            Vertex e1 = Vertex.ByCoordinates(10, 0, 0);
            Vertex e2 = Vertex.ByCoordinates(10, 10, 0);
            Vertex f1 = a1;
            Vertex f2 = b1;
            var a = Edge.ByStartVertexEndVertex(a1, a2); 
            var b = Edge.ByStartVertexEndVertex(b1, b2); 
            var c = Edge.ByStartVertexEndVertex(c1, c2);
            var d = Edge.ByStartVertexEndVertex(d1, d2);
            var e = Edge.ByStartVertexEndVertex(e1, e2);
            var f = Edge.ByStartVertexEndVertex(f1, f2);
            var g = Edge.ByStartVertexEndVertex(a1, e1);
            var h = Edge.ByStartVertexEndVertex(b1, Vertex.ByCoordinates(2.5, 5, 0));
            var xaligned = Edge.ByStartVertexEndVertex(a1, Vertex.ByCoordinates(114.750, 0, 0));
            var xCoincident = Edge.ByStartVertexEndVertex(e1, b1);

            var side = Edge.ByStartVertexEndVertex(
                    Vertex.ByCoordinates(-17.950, 31.090, 0) , Vertex.ByCoordinates(-7.650, 48.930,0)
                );
            var rayEdge = Edge.ByStartVertexEndVertex(
                    Vertex.ByCoordinates(-49.544, 39.031, 0), Vertex.ByCoordinates(5827.960, 39.031, 0)
                );

            var vertical = Edge.ByStartVertexEndVertex(a1, b1);
            var otherRay = Edge.ByStartVertexEndVertex(
                    Vertex.ByCoordinates(0, 5, 0), Vertex.ByCoordinates(15, 5, 0)
                );

            var coincidentProjection1 = Edge.ByStartVertexEndVertex(
                    Vertex.ByCoordinates(35, 30),
                    Vertex.ByCoordinates(13.571429, 17.142857)
                );
            var coincidentProjection2 = Edge.ByStartVertexEndVertex(
                    Vertex.ByCoordinates(17, 28),
                    Vertex.ByCoordinates(8, 25.5)
                );

            Geometry ab = a.Intersection(b); // Intersecting
            Geometry ac = a.Intersection(c); // Skew edges
            Geometry ad = a.Intersection(d); // Coplanar but not intersecting
            Geometry ef = e.Intersection(f); // Coplanar and parallel
            Geometry gh = g.Intersection(h); // Coplanar, not intersecting and second edge shorter than first

            //Assert.NotNull(ab);
            //Assert.AreEqual(5, (ab as Vertex).X);
            //Assert.AreEqual(5, (ab as Vertex).Y);
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