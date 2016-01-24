using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.Waves;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Unit.Modules.Waves
{

    [TestClass]
    public class Waves_Params_Test
    {
        [TestMethod]
        [Description("Test if wavelet type is initialized")]
        public void WaveletTest1()
        {

            int decomp = 3;
            int step = 500;
            Wavelet_Type wave_type = Wavelet_Type.haar;

            Waves_Params param = new Waves_Params(wave_type, decomp, "Analysis1",step);

            Assert.AreEqual(param.WaveType, wave_type);
        }
        

        [TestMethod]
        [Description("Test if decomposition level is initialized")]
        public void DecompTest1()
        {
            int decompLevel = 3;
            int step = 500;

            Waves_Params param = new Waves_Params(Wavelet_Type.haar, decompLevel, "Analysis1", step);

            Assert.AreEqual(param.DecompositionLevel, decompLevel);
        }

        [TestMethod]
        [Description("Test if decomposition level is set properly")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Decomposition level below 1")]
        public void DecompTest2()
        {
            int decompLevel = -1;
            int step = 500;

            Waves_Params param = new Waves_Params(Wavelet_Type.haar, decompLevel, "Analysis1", step);
        }

        [TestMethod]
        [Description("Test if R-peaks step size is initialized")]
        public void StepTest1()
        {
            int decompLevel = 3;
            int step = 500;

            Waves_Params param = new Waves_Params(Wavelet_Type.haar, decompLevel, "Analysis1", step);

            Assert.AreEqual(param.RpeaksStep, step);
        }

        [TestMethod]
        [Description("Test if R-peaks step size is set properly")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "R-peaks step size below 1")]
        public void StepTest2()
        {
            int decompLevel = 3;
            int step = -1;

            Waves_Params param = new Waves_Params(Wavelet_Type.haar, decompLevel, "Analysis1", step);
        }
    }
}
