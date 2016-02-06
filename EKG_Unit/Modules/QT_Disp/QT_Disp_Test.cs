using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.QT_Disp;
using EKG_Project.IO;
using EKG_Unit;
using EKG_Project.Modules;


namespace EKG_Unit.Modules.QT_Disp
{
    [TestClass]
    public class QT_Disp_Test
    {
        [TestMethod]  
            [Description("Test if interface stops after Abort()")]  
            public void iterativeInterfaceTest1()
            {  
                IModule testModule = new EKG_Project.Modules.QT_Disp.QT_Disp();

            //QT_Calc_Method method_qt_calc = QT_Calc_Method.BAZETTA;
            //T_End_Method method_t_end = T_End_Method.TANGENT;
            QT_Disp_Params param = new QT_Disp_Params("abc123");  
      
                testModule.Init(param);  
                int counter = 0;  
              while (!testModule.Ended()) 
              { 
                  counter++; 
                  Console.WriteLine(testModule.Progress()); 
                  testModule.ProcessData(); 
                   testModule.Abort();  
               }  
               Assert.AreEqual(1, counter);  
        }  

    }
}
