using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using TestServices;

namespace TemporaryTests
{
    //[TestFixture]
    public class Tests : GeometricTestBase
    {
        //[Test]
        public void NotNullTest()
        {
            //var myObject = Point.ByCoordinates(5, 5, 5);
            Assert.NotNull(1);
        }
    }
}