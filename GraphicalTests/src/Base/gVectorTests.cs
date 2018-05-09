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
    public class gVectorTests
    {
        [Test]
        public void ByCoordinatesTest()
        {
            gVector v = gVector.ByCoordinates(0, 4, 3);
            Assert.AreEqual(0, v.X);
            Assert.AreEqual(4, v.Y);
            Assert.AreEqual(3, v.Z);
            Assert.AreEqual(5, v.Length);
        }

        [Test]
        public void ByTwoVerticesTest()
        {
            gVertex vertex1 = gVertex.ByCoordinates(10, 10, 10);
            gVertex vertex2 = gVertex.ByCoordinates(10, 14, 13);
            gVector v = gVector.ByTwoVertices(vertex1, vertex2);
            Assert.AreEqual(0, v.X);
            Assert.AreEqual(4, v.Y);
            Assert.AreEqual(3, v.Z);
            Assert.AreEqual(5, v.Length);
        }

        [Test]
        public void DotTest()
        {
            var vectors = TestVectorPair();
            var v1 = vectors[0];
            var v2 = vectors[1];
            var v3 = vectors[2];
            Assert.AreEqual(100, v1.Dot(v2));
            Assert.AreEqual(0, v1.Dot(v3));
        }

        [Test]
        public void AngleTest()
        {
            var vectors = TestVectorPair();
            var v1 = vectors[0];
            var v2 = vectors[1];
            var v3 = vectors[2];
            var v4 = gVector.ByCoordinates(-10, 0, 0);
            var v5 = gVector.ByCoordinates(10, 0.00001, 0);
            var v6 = gVector.ByCoordinates(10, 0.0000001, 0);
            Assert.IsTrue(gBase.Threshold(v1.Angle(v2), 45.0));
            Assert.IsTrue(gBase.Threshold(v1.Angle(v3), 90.0));
            Assert.IsFalse(gBase.Threshold(v4.Angle(v5), 180.0)); // Floating point threshold
            Assert.IsTrue(gBase.Threshold(v4.Angle(v6), 180.0)); // Floating point threshold
        }

        [Test]
        public void CrossTest()
        {
            // Cross vectors copmpared with https://www.wolframalpha.com/input/?i=(0,+10,+10)+cross+(34,+100,+43.2)
            var vectors = TestVectorPair();
            var v1 = vectors[0];
            var v2 = vectors[1];
            var v3 = gVector.ByCoordinates(34, 100, 43.2);
            var cross12 = v1.Cross(v2);
            var cross13 = v1.Cross(v3);

            Assert.AreEqual(-100, cross12.X);
            Assert.AreEqual(0, cross12.Y);
            Assert.AreEqual(100.0, Math.Round(cross12.Length,3));
            Assert.AreEqual(-568, cross13.X);
            Assert.AreEqual(340, cross13.Y);
            Assert.AreEqual(-340, cross13.Z);
            Assert.AreEqual(744.194, Math.Round(cross13.Length, 3));
            
        }

        [Test]
        public void IsParallelToTest()
        {
            var a = gVector.ByTwoVertices(gVertex.ByCoordinates(0, 0, 0), gVertex.ByCoordinates(0, 10, 10));
            var b = gVector.ByTwoVertices(gVertex.ByCoordinates(10, 0, 0), gVertex.ByCoordinates(10, 10, 10));
            var c = gVector.ByCoordinates(5, 15, 10);
            Assert.IsTrue(a.IsParallelTo(b));
            Assert.IsFalse(a.IsParallelTo(c));
        }

        public gVector[] TestVectorPair()
        {
            var a = gVector.ByCoordinates(0, 10, 10);
            var b = gVector.ByCoordinates(0, 10, 0);
            var c = gVector.ByCoordinates(0, 10, -10);
            return new gVector[] { a, b, c };
        }

    }
}