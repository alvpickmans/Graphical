using NUnit.Framework;
using Graphical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicalTests.CustomObject
{
    [TestFixture]
    public class UserDataTests
    {

        [Test]
        public void GetDataTEst()
        {
            var userData = new UserData();
            userData["name"] = "alvaro";
            userData["id"] = 1;


            string name = userData.GetValueOrDefault<string>("name");
            int id = userData.GetValueOrDefault<int>("id");
            int wrongIdKey = userData.GetValueOrDefault<int>("name");
            string notExisting = userData.GetValueOrDefault<string>("notExisting");

            Assert.AreEqual(name, "alvaro");
            Assert.AreEqual(id, 1);
            Assert.AreEqual(wrongIdKey, default(int));
            Assert.AreEqual(notExisting, default(string));
        }

    }
}
