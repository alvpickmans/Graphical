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
            Vertex vertex = Vertex.Origin();
            int id = vertex.Id;
            for(var i = 0; i < 100; i++)
            {
                vertex = Vertex.Origin();
            }

            Assert.AreEqual(vertex.Id, id + 100);
        }

    }
}