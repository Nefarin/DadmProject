using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.QT_Disp;

namespace EKG_Unit.Modules.QT_Disp
{
    [TestClass]
    public class QT_Disp_Params_Test
    {
        [TestMethod]
        [Description("Check init")]
        public void testParamsInit()
        {
            QT_Calc_Method method_qt_calc = QT_Calc_Method.BAZETTA;
            T_End_Method method_t_end = T_End_Method.TANGENT;
            bool drains = false;
            QT_Disp_Params parameters = new QT_Disp_Params("Test");
            Assert.AreEqual(parameters.AllDrains, drains);
            Assert.AreEqual(parameters.AnalysisName, "Test");
            Assert.AreEqual(parameters.QTMethod, method_qt_calc);
            Assert.AreEqual(parameters.TEndMethod, method_t_end);            
        }
        [TestMethod]
        [Description("Check T_End_Method assert")]
        public void testT_End_method()
        {
            QT_Disp_Params param = new QT_Disp_Params("Test");
            param.TEndMethod = T_End_Method.PARABOLA;            
            Assert.AreEqual(param.TEndMethod, T_End_Method.PARABOLA);             
        }
        [TestMethod]
        [Description("Check QT_Calc_Method assert")]
        public void testQT_Calc_Method()
        {
            QT_Disp_Params param = new QT_Disp_Params("Test");
            param.QTMethod = QT_Calc_Method.FRAMIGHAMA;
            Assert.AreEqual(param.QTMethod, QT_Calc_Method.FRAMIGHAMA);
            param.QTMethod = QT_Calc_Method.FRIDERICA;
            Assert.AreEqual(param.QTMethod, QT_Calc_Method.FRIDERICA);
            param.QTMethod = QT_Calc_Method.BAZETTA;
            Assert.AreEqual(param.QTMethod, QT_Calc_Method.BAZETTA);
        }
        [TestMethod]
        [Description("Check alldrains assert")]
        public void testAlldrains()
        {
            QT_Disp_Params param = new QT_Disp_Params("Test");
            param.AllDrains = true;
            Assert.AreEqual(param.AllDrains, true);
            param.AllDrains = false;
            Assert.AreEqual(param.AllDrains, false);
        }

    }
}
