using Graphical.Geometry;
using Graphical.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Graphical.Geometry.Tests
{
    [TestFixture]
    public class RayTests
    {
        [Test]
        public void IntersectionOffset_Pass_WhenVertexIsCoincidentInOrigin()
        {
            Ray ray = Ray.XAxis(Vertex.Origin());

            double offset = ray.IntersectionOffset(Vertex.Origin());

            Assert.AreEqual(0, offset);
        }

        [Test]
        public void IntersectionOffset_Pass_WhenCoincidentInOrigin()
        {
            Ray ray = Ray.XAxis(Vertex.Origin());
            Edge edge = Edge.ByCoordinatesArray(new double[6] { 0, -5, 0, 0, 5, 0 });

            double offset = ray.IntersectionOffset(edge);

            Assert.AreEqual(0, offset);            
        }

        [Test]
        public void IntersectionOffset_Pass_WhenEdgeIsAtFront()
        {
            Ray ray = Ray.ByTwoVertices(
                Vertex.ByCoordinates(5,5,0),
                Vertex.ByCoordinates(10,10,0)
            );
            Edge edge = Edge.ByCoordinatesArray(new double[6] { 15, 30, 0, 15, 0, 0 });
            Vertex expected = Vertex.ByCoordinates(15, 15, 0);

            double offset = ray.IntersectionOffset(edge);            
            Vertex v = ray.Origin.Translate(ray.Direction.Scale(offset));

            Assert.IsTrue(offset > 0);
            Assert.AreEqual(expected, v);

        }

        [Test]
        public void IntersectionOffset_Pass_WhenEdgeIsAtBack()
        {
            Ray ray = Ray.ByTwoVertices(
                Vertex.ByCoordinates(5, 5, 0),
                Vertex.ByCoordinates(10, 10, 0)
            );
            Edge edge = Edge.ByCoordinatesArray(new double[6] { -15, 30, 0, -15, 0, 0 });
            Vertex expected = Vertex.ByCoordinates(-15, -15, 0);

            double offset = ray.IntersectionOffset(edge);
            Vertex v = ray.Origin.Translate(ray.Direction.Scale(offset));

            Assert.IsTrue(offset < 0);
            Assert.AreEqual(expected, v);

        }

        [Test]
        public void IntersectionOffset_Pass_WhenParallelIsPositiveInfinite()
        {
            Ray ray = Ray.ByTwoVertices(
                Vertex.ByCoordinates(5, 5, 0),
                Vertex.ByCoordinates(10, 10, 0)
            );
            Edge edge = Edge.ByCoordinatesArray(new double[6] { 5, 10, 0, 10, 15, 0 });

            double offset = ray.IntersectionOffset(edge);

            Assert.IsTrue(Double.IsInfinity(offset));
        }


        [Test]
        public void IntersectionOffset_Pass_WhenColinearIsInfinite()
        {
            Ray ray = Ray.XAxis(Vertex.Origin());
            Edge edge = Edge.ByCoordinatesArray(new double[6] { -5, 2, 0, 5, 2, 0 });

            double offset = ray.IntersectionOffset(edge);

            Assert.IsTrue(Double.IsInfinity(offset));
        }

        [Test]
        public void Intersection()
        {
            var ray = Ray.ZAxis(Vertex.ByCoordinates(5,0,0));
            var edge = Edge.ByCoordinatesArray( new double[6] { 5, 2, 3, 5, -2, 3 });
            var intersection = ray.Intersection(edge);
            var expected = Vertex.ByCoordinates(5, 0, 3);

            Assert.AreEqual(expected, intersection);

            var edgeIntersection = Ray.XAxis(Vertex.Origin())
                .Intersection(Edge.ByCoordinatesArray(new double[] { -5, 0, 0, 5, 0, 0 }));

            Assert.IsAssignableFrom<Edge>(edgeIntersection);

            var parallelNotIntersecting = Ray.YAxis(Vertex.ByCoordinates(0, 2, 0))
                .Intersection(Edge.ByCoordinatesArray(new double[] { 5, 2, 0, 5, 10, 0 }));

            Assert.IsNull(parallelNotIntersecting);

        }
    }
}
