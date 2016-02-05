using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Heart_Axis;
using System.Reflection;


namespace EKG_Unit.Modules.Heart_Axis
{
    [TestClass]
    public class Heart_Axis_Alg_Test
    {
        [TestMethod]
        [Description("Test if pseudomodule works properly")]
        public void pseudomoduleTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 3, 4, 3, 4, 3 };
            double[] resultArray = { 5, 5, 5, 5 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            double[] realresultArray = testAlgs.PseudoModule(0, 4, testArray);

            CollectionAssert.AreEquivalent(realresultArray, resultArray);

        }

        [TestMethod]
        [Description("Test if method finds maximum")]
        public void maxOfPsudomoduleTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 1, 2, 8, 1, 6 };
            int result = 2;


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            int realresult = testAlgs.MaxOfPseudoModule(0, testArray);

            Assert.AreEqual(realresult, result);

        }

        [TestMethod]
        [Description("Test if method finds maximum when all arguments are equal")]
        public void maxOfPsudomoduleTest2()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 2, 2, 2, 2, 2 };
            int result = 0;


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            int realresult = testAlgs.MaxOfPseudoModule(0, testArray);

            Assert.AreEqual(realresult, result);

        }

        [TestMethod]
        [Description("LeastSquaresMethod")] 
        public void leastSquaresMethodTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 4, 7, 8, 7, 4 };
            double[] testArray2 = { 1, 1, 8, 1, 1 };
            double[] resultArray = { 4, 4, -1 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            double[] realresultArray = testAlgs.LeastSquaresMethod(testArray, 0, testArray2, 100);

            CollectionAssert.AreNotEqual(realresultArray, resultArray);

        }

        [TestMethod]
        [Description("LeastSquaresMethod - when signal is too short")]
        public void leastSquaresMethodTest2()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 4, 7, 8, 7, 4 };
            double[] testArray2 = { 5, 1, 2, 1, 1 };
            double[] resultArray = { 0, 0, 0};


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            double[] realresultArray = testAlgs.LeastSquaresMethod(testArray, 0, testArray2, 100);

            CollectionAssert.AreEquivalent(realresultArray, resultArray);

        }

        [TestMethod]
        [Description("Test if method MaxOfPolynomial calculates properly")]
        public void maxOfPolynomialTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 12, 8, -4 };
            int result = 1;


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            int realresult = testAlgs.MaxOfPolynomial(0, testArray);
            Assert.AreEqual(realresult, result);

        }

        [TestMethod]
        [Description("MaxOfPolynomial - dividing by 0")]
        public void maxOfPolynomialTest2()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 1, 8, 0 };
            int result = 0;


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            int realresult = testAlgs.MaxOfPolynomial(0, testArray);
            Assert.AreEqual(realresult, result);

        }


        [TestMethod]
        [Description("ReadingAmplitudes")]
        public void readingAmplitudesTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray1 = { 5, 4, 3, 2, 1 };
            double[] testArray2 = { 1, 2, 3, 4, 5 };
            double[] resultArray = { 4, 2 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            double[] realresultArray = testAlgs.ReadingAmplitudes(testArray1, testArray2, 1);


            CollectionAssert.AreEquivalent(realresultArray, resultArray);

        }

        [TestMethod]
        [Description("ReadingAmplitudes - while index equals 0")]
        public void readingAmplitudesTest2()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray1 = { 5, 4, 3, 2, 1 };
            double[] testArray2 = { 1, 2, 3, 4, 5 };
            double[] resultArray = { 0, 0 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            double[] realresultArray = testAlgs.ReadingAmplitudes(testArray1, testArray2, 0);


            CollectionAssert.AreEquivalent(realresultArray, resultArray);

        }


        [TestMethod]
        [Description("IandII")]
        public void IandIITest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 2, 5};
            double result = 1.047;


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testArray };
            obj.Invoke("IandII", args);
            double realresult = Convert.ToDouble(obj.Invoke("IandII", args));
            Assert.AreEqual(result, realresult, 0.001);

        }

        [TestMethod]
        [Description("IandII - dividing by 0")]
        public void IandIITest2()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");
            double[] testArray = { 0, 2 };
            double result = 0;
            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            double realresult = testAlgs.IandII(testArray);

            Assert.AreEqual( result, realresult);
        }

        }
}
