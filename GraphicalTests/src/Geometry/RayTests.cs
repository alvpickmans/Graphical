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
    public class RayTests
    {
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
