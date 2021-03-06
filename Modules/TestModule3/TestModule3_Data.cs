﻿using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.TestModule3
{
    public class TestModule3_Data : ECG_Data
    {
        private uint _frequency;
        private uint _sampleAmount;
        private Vector<double> _output;

        public uint Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        public uint SampleAmount
        {
            get
            {
                return _sampleAmount;
            }
            set
            {
                _sampleAmount = value;
            }
        }

        public Vector<double> Output
        {
            get
            {
                return _output;
            }

            set
            {
                _output = value;
            }
        }

        public TestModule3_Data()
        {

        }
        public TestModule3_Data(uint frequency, uint sampleAmount) : this()
        {
            Frequency = frequency;
            SampleAmount = sampleAmount;
        }
    }
}
