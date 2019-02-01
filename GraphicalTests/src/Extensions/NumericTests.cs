using NUnit.Framework;
using Graphical.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Extensions.Tests
{
    [TestFixture]
    public class NumericTests
    {
        [Test]
        [Category("Unit Tests")]
        public void MapTest()
        {
            Assert.AreEqual(0.5, Convert.ToDouble(15).Map(10,20,0,1));
            Assert.AreEqual(25, Convert.ToDouble(0.5).Map(0, 1, 0, 50));
        }
    }
}