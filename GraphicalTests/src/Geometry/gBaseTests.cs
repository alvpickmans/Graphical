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
    public class gBaseTests
    {
        [Test]
        public void RoundTest()
        {
            Assert.AreEqual(648.000, (648.00000000000023).Round());
        }
    }
}