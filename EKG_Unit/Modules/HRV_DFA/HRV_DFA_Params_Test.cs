using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV_DFA;


namespace EKG_Unit.Modules.HRV_DFA
{
    [TestClass]
    public class HRV_DFA_Params_Test
    {
        [TestMethod]
        [Description("Test if null is given as analysis name")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HRV_DFA_ParamsTest()
        {
            string testString = null;
            HRV_DFA_Params param = new HRV_DFA_Params(testString);
        }
    }
}
