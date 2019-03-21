using Graphical.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Extensions;
using NUnit.Framework;

namespace Graphical.Geometry.Tests
{
    [TestFixture]
    public class GeometryTests
    {
        [Test]
        public void RoundTest()
        {
            Assert.AreEqual(648.000, (648.00000000000023).Round());
        }

        [Test]
        public void GeometryIdUniqueTest()
        {
            int id = 0;
            for(var i = 0; i < 100; i++)
            {
                id = Geometry.NextId;
            }

            Assert.AreEqual(id, 100);
        }

        [Test]
        public void GeometryEqualityTest()
        {
            var edge1 = gEdge.ByStartVertexEndVertex(Vertex.Origin(), Vertex.ByCoordinates(5, 5, 5));
            var edge2 = gEdge.ByStartVertexEndVertex(Vertex.Origin(), Vertex.ByCoordinates(5, 5, 5));

            Assert.IsFalse(edge1.Equals(edge2));

        }
    }
}