using NUnit.Framework;
using Graphical.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Core.Tests
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        [Category("Unit Tests")]
        public void MapTest()
        {
            Assert.AreEqual(0.5, List.Map(15, 10,20,0,1));
            Assert.AreEqual(25, List.Map(0.5, 0, 1, 0, 50));
        }
    }
}