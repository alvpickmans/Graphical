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
            var e2 = gVertex.ByCoordinates(5, 0.1, 0);

            Assert.IsTrue(b.OnEdge(a, b)); // Same end point
            Assert.IsTrue(b.OnEdge(a, c)); // On edge
            Assert.IsFalse(c.OnEdge(a, b)); // Colinear but not in between.
            Assert.IsFalse(d.OnEdge(a, c)); // No Colinear
            Assert.IsTrue(e.OnEdge(a, a2)); // Almost colinear, smaller than threshold
            Assert.IsFalse(e2.OnEdge(a, a2)); // Almost colinear, bigger than threshold
        }

        [Test]
        public void CoplanarTest()
        {
            var a = gVertex.ByCoordinates(0, 0, 0);
            var b = gVertex.ByCoordinates(0, 10, 0);
            var c = gVertex.ByCoordinates(10, 10, 0);
            var d = gVertex.ByCoordinates(15, 25, 0);
            var e = gVertex.ByCoordinates(0, 50, 0);
            var f = gVertex.ByCoordinates(15, 15, 0.5);
            
            Assert.IsTrue(gVertex.Coplanar(new List<gVertex>() { a, b}));
            Assert.IsTrue(gVertex.Coplanar(new List<gVertex>() { a, b, c }));
            Assert.IsFalse(gVertex.Coplanar(new List<gVertex>() { a, b, c, d, e, f}));
            Assert.IsFalse(gVertex.Coplanar(new List<gVertex>() { c, d, e, f}));
        }
        
    }
}