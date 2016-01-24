using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.Statistics;

namespace EKG_Project.Modules.HRV2
{

    public partial class HRV2_Alg
    {
        private const float binLength = 7.8125f;

        //public Histogram2 makeHistogram()
        //{
        //    Vector<double> RRIntervals = InputData.RRInterval[_outputIndex].Item2;
        //    //int binAmount = (int)((RRIntervals.AbsoluteMaximum() - RRIntervals.AbsoluteMinimum()) / 7.8125); //the amount of the bins
        //    Console.WriteLine(RRIntervals.Max());
        //    Console.ReadLine();
        //    // _currentHistogram = new Histogram(RRIntervals, binAmount);
        //    _currentHistogram = new Histogram2(binLength, RRIntervals);
        //    return _currentHistogram;
        //}
        
        public class Sample
        {
            public int Count { get; set; }
            public float LowestValue { get; set; }
            public float HighestValue { get; set; }
            public float AverageValue
            {
                get
                {
                    return (LowestValue + HighestValue) / 2;
                }
            }

            public override string ToString()
            {
                return String.Format("Average value: {0}, lowest value: {1}, highest value: {2}, count: {3}",
                    AverageValue,
                    LowestValue,
                    HighestValue,
                    Count);
            }
        }

        public class Histogram2
        {
            private int maxCount;
            public int MaxCount
            {
                get
                {
                    if (maxCount == 0)
                    {
                        maxCount = _samples.Max(p => p.Count);
                    }
                    return maxCount;
                }
            }

            private Vector<double> INPUT;
            private double _width;
            private ObservableCollection<Sample> _samples;

            public Histogram2(double width, Vector<double> input)
            {
                this.INPUT = input;
                _width = width;
                _samples = new ObservableCollection<Sample>();
                //ensureDataLoaded();
            }

            public Histogram2()
            {
            }

            public ObservableCollection<Sample> Samples
            {
                get
                {
                    if (_samples.Count == 0)
                    {
                        ensureDataLoaded();
                    }
                    return _samples;
                }
            }

            private void ensureDataLoaded()
            {
                if (INPUT != null && INPUT.Count != 0)
                {
                    groupSamples(INPUT);
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }

            private void groupSamples(Vector<double> samples)
            {
                List<double> temp = new List<double>(samples.ToArray());
                groupSamples(temp);
            }

            private void groupSamples(List<Double> samples)
            {
                double start = samples[0];
                List<List<Double>> _helperListOfSampleList = new List<List<Double>>();
                List<Double> _helperSampleList = new List<Double>();
                foreach (Double d in samples)
                {
                    _helperSampleList.Add(d);
                    if (d >= start + _width)
                    {
                        _helperListOfSampleList.Add(_helperSampleList);
                        _helperSampleList = new List<Double>();
                        start = d;
                    }
                }
                try
                {
                    //checkCountOfSamples(_helperListOfSampleList, samples);
                    createSampleList(_helperListOfSampleList);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            private void createSampleList(List<List<Double>> lists)
            {
                if (lists == null)
                    return;

                foreach (List<Double> l in lists)
                {
                    if (l == null)
                        return;

                    Sample sample = new Sample()
                    {
                        Count = l.Count,
                        LowestValue = (float)l[0],
                        HighestValue = (float)l[l.Count - 1]
                    };
                    _samples.Add(sample);
                }
            }

            #region Debug functions

            [Conditional("DEBUG")]
            void checkCountOfSamples(List<List<Double>> listOfSampleList, List<Double> sampleList)
            {
                int sum = 0;
                foreach (List<Double> l in listOfSampleList)
                {
                    sum += l.Count;
                }

                if (sum != sampleList.Count)
                {
                    throw new System.Exception("Error, error");
                }
            }

            #endregion
        }
    }
}

