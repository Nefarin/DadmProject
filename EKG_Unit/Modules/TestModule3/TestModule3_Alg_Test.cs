using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.TestModule3;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Unit.Modules.TestModule3
{
    [TestClass]
    public class TestModule3_Alg_Test
    {
        [TestMethod]
        [Description("Test if vector properly scales - equality test")]
        public void scaleSamplesTest1()
        {
            // Init test here
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] resultArray = { 2, 4, 6, 8, 10 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);

            // Since we want to test private method there is needed another overhead - public function does not need this
            // can be invoked in normal way

            PrivateObject obj = new PrivateObject(testAlgs);

            // Process test here

            obj.Invoke("scaleSamples");

            // Assert results

            Assert.AreEqual(testAlgs.CurrentVector, resultVector);
            

        }

        [TestMethod]
        [Description("Test if vector properly scales - not equality test")]
        public void scaleSamplesTest2()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] resultArray = { 2, 4, 5, 8, 10 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            obj.Invoke("scaleSamples");

            Assert.AreNotEqual(testAlgs.CurrentVector, resultVector);


        }

        [TestMethod]
        [Description("Test if vector adds properly - equality test")]
        public void addVectorTest1()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] addArray = { 1, 1, 1, 1, 1 };
            double[] resultArray = { 2, 3, 4, 5, 6 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> addVector = Vector<double>.Build.DenseOfArray(addArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { addVector }; // shows how to pass parameters to private function
            obj.Invoke("addVector", args);

            Assert.AreEqual(testAlgs.CurrentVector, resultVector);


        }

        [TestMethod]
        [Description("Test if vector adds properly - not equality test")]
        public void addVectorTest2()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] addArray = { 1, 1, 1, 1, 1 };
            double[] resultArray = { 2, 3, 4, 5, 5 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> addVector = Vector<double>.Build.DenseOfArray(addArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { addVector };
            obj.Invoke("addVector", args);

            Assert.AreNotEqual(testAlgs.CurrentVector, resultVector);


        }

        [TestMethod]
        [Description("Test if add throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void addVectorTest3()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] addArray = { 1, 1, 1, 1, 1 };
            double[] resultArray = { 2, 3, 4, 5, 5 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> addVector = Vector<double>.Build.DenseOfArray(addArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { null };
            obj.Invoke("addVector", args);
        }

        [TestMethod]
        [Description("Test if vector subs properly - equality test")]
        public void subVectorTest1()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] subArray = { 1, 1, 1, 1, 1 };
            double[] resultArray = { 0, 1, 2, 3, 4 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> subVector = Vector<double>.Build.DenseOfArray(subArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { subVector };
            obj.Invoke("subVector", args);

            Assert.AreEqual(testAlgs.CurrentVector, resultVector);


        }

        [TestMethod]
        [Description("Test if vector subs properly - not equality test")]
        public void subVectorTest2()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] subArray = { 1, 1, 1, 1, 1 };
            double[] resultArray = { 0, 0, 0, 0, 0 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> subVector = Vector<double>.Build.DenseOfArray(subArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { subVector };
            obj.Invoke("subVector", args);

            Assert.AreNotEqual(testAlgs.CurrentVector, resultVector);


        }

        [TestMethod]
        [Description("Test if sub throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void subVectorTest3()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");

            double[] testArray = { 1, 2, 3, 4, 5 };
            double[] subArray = { 1, 1, 1, 1, 1 };
            double[] resultArray = { 2, 3, 4, 5, 5 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> subVector = Vector<double>.Build.DenseOfArray(subArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { null };
            obj.Invoke("addVector", args);
        }

        [TestMethod]
        [Description("Test if constructor throws null if arguments are not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void constructorTest1()
        {
            TestModule3_Params testParams = null;
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
        }

        [TestMethod]
        [Description("Test if constructor throws null if arguments are not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void constructorTest2()
        {
            TestModule3_Params testParams = new TestModule3_Params(2, 500, "Test");
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = null;
            TestModule3_Alg testAlgs = new TestModule3_Alg(testVector, testParams);
        }

        public static void Main()
        {

        }
    }
}
