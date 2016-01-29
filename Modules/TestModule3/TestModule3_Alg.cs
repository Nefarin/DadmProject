using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using EKG_Project.IO;

namespace EKG_Project.Modules.TestModule3
{
    public class TestModule3_Alg
    {
        private Vector<Double> _currentVector;
        private TestModule3_Params _params;

        public TestModule3_Params Params
        {
            get
            {
                return _params;
            }

            set
            {
                if (value == null) throw new ArgumentNullException();
                _params = value;
            }
        }

        public Vector<double> CurrentVector
        {
            get
            {
                return _currentVector;
            }

            set
            {
                if (value == null) throw new ArgumentNullException();
                _currentVector = value;
            }
        }

        public TestModule3_Alg(Vector<Double> data, TestModule3_Params param)
        {
            CurrentVector = data; // pracujemy na referencjach, nie tworzymy nowego, cięcie ma następować w interfejsie (od środy/czwartku)
            Params = param; // parametry są lekkie, ich referncja dla wygody może być też tutaj
        }

        public void scaleSamples()
        {
            CurrentVector = CurrentVector.Multiply(Params.Scale);
        }

        public void addVector(Vector<Double> toAdd)
        {
            if (toAdd == null) throw new ArgumentNullException();
            CurrentVector = CurrentVector.Add(toAdd);
        }

        public void subVector(Vector<Double> toSub)
        {
            if (toSub == null) throw new ArgumentNullException();
            CurrentVector = CurrentVector.Subtract(toSub);
        }
    }
}
