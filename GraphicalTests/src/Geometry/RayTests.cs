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
            var ray = Ray.XAxis(Vertex.Origin());
            var edge = Edge.ByStartVertexEndVertex(
                    Vertex.ByCoordinates(4, 1, 2),
                    Vertex.ByCoordinates(6, -1, 2)
                );

            var intersection = ray.Intersection(edge);

            var expected = Vertex.ByCoordinates(5, 0, 0);

            Assert.IsNull(intersection);

        }
    }
}
