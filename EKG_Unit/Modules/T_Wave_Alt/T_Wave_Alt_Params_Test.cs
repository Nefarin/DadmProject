using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.T_Wave_Alt;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Unit.Modules.T_Wave_Alt
{
    [TestClass]
    public class T_Wave_Alt_Params_Test
    {
        [TestMethod]
        [Description("Tests if analysis name is set correctly")]
        public void analysisNameTest()
        {
            T_Wave_Alt_Params testParams = new T_Wave_Alt_Params("testName");
            string expectedName = "testName";

            Assert.AreEqual(expectedName, testParams.AnalysisName);
        }

        [TestMethod]
        [Description("Tests if analysis name is set as default if it's not defined")]
        public void emptyAnalysisNameTest()
        {
            T_Wave_Alt_Params testParams = new T_Wave_Alt_Params();
            string expectedName = "undefined";

            Assert.AreEqual(expectedName, testParams.AnalysisName);
        }
    }
}
