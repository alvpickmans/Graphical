using System;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

using NUnit.Framework;

using TestServices;

namespace SampleLibraryTests
{

    [TestFixture]
    [IsVisibleInDynamoLibrary(false)]
    class HelloDynamoZeroTouchTests : GeometricTestBase
    {
        //[Test]
        public void PassingTest()
        {
            var myObject = 2;
            Assert.NotNull(myObject);
        }

        //[Test]
        public void FailingTest()
        {
            var p1 = Point.ByCoordinates(0, 0, 0);
            var p2 = Point.ByCoordinates(0, 0, 0);
            Assert.Throws<ApplicationException>(()=>Line.ByStartPointEndPoint(p1,p2));
        }
    }
}