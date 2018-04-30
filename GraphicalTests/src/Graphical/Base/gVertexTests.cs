using NUnit.Framework;
using Graphical.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Base.Tests
{
    [TestFixture]
    public class gVertexTests
    {
        [Test]
        public void OnEdgeTest()
        {
            var a = gVertex.ByCoordinates(0, 0, 0);
            var a2 = gVertex.ByCoordinates(10, 0, 0);
            var b = gVertex.ByCoordinates(5, 5, 5);
            var c = gVertex.ByCoordinates(10, 10, 10);
            var d = gVertex.ByCoordinates(3, 4, 6);
            var e = gVertex.ByCoordinates(5, 0.00001, 0);
            var e2 = gVertex.ByCoordinates(5, 0.01, 0);

            //Assert.IsTrue(b.OnEdge(a, b)); // Same end point
            //Assert.IsTrue(b.OnEdge(a, c)); // On edge
            //Assert.IsFalse(c.OnEdge(a, b)); // Colinear but not in between.
            //Assert.IsFalse(d.OnEdge(a, c)); // No Colinear
            //Assert.IsTrue(e.OnEdge(a, a2)); // Almost colinear, smaller than threshold
            Assert.IsFalse(e2.OnEdge(a, a2)); // Almost colinear, bigger than threshold
        }
        
    }
}