using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Heart_Axis;


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
            object[] args = {0, 4, testArray};
            obj.Invoke("PseudoModule", args);

            Assert.AreNotEqual(testAlgs, resultArray);

        }

        [TestMethod]
        [Description("Test if method finds maximum")]
        public void maxOfPsudomoduleTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 1, 2, 8, 1, 6 };
            double[] resultArray = { 2 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { 0, testArray };
            obj.Invoke("MaxOfPseudoModule", args);

            Assert.AreNotEqual(testAlgs, resultArray);

        }

        [TestMethod]
        [Description("LeastSquaresMethod")] //todo
        public void leastSquaresMethodTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 1, 2, 8, 1, 6 };
            double[] resultArray = { 2 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { 0, testArray };
            obj.Invoke("MaxOfPseudoModule", args);

            Assert.AreNotEqual(testAlgs, resultArray);

        }

        [TestMethod]
        [Description("Test if method MaxOfPolynomial calculates properly")]
        public void maxOfPolynomialTest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { -4, 8, 12 };
            double[] resultArray = { 1 };


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { 0, testArray };
            obj.Invoke("MaxOfPolynomial", args);

            Assert.AreNotEqual(testAlgs, resultArray);

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
            object[] args = { testArray1, testArray2, 1 };
            obj.Invoke("ReadingAmplitudes", args);

            Assert.AreNotEqual(testAlgs, resultArray);

        }


        [TestMethod]
        [Description("IandII")]
        public void IandIITest()
        {
            Heart_Axis_Params testParams = new Heart_Axis_Params("Test");

            double[] testArray = { 2, 4};
            double result = 1.357;


            Heart_Axis_Alg testAlgs = new Heart_Axis_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { 0, testArray };
            obj.Invoke("MaxOfPseudoModule", args);

            Assert.AreEqual(testAlgs, result, "0.2"); //?

        }

        [TestMethod]
        [Description("IandII")]
        public void IandIITest2()
        {
            //dividing by 0?
        }

        }
}
