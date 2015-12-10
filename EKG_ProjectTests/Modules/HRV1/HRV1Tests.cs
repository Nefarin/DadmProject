using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.HRV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV1.Tests
{
    [TestClass()]
    public class HRV1Tests
    {
        [TestMethod()]
        public void lombScargleTest()
        {
            int i = 1;
            if (i == 1) {
                Assert.Fail();
            }
            Assert.Fail();
        }

        public static void Main()
        {
            HRV1Tests hrvT = new HRV1Tests();
            hrvT.lombScargleTest();
        }
    }
}