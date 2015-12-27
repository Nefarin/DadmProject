using Histogram.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Histogram.Data
{
    public class Sample
    {
        public float Count { get; set; }
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

    public class DataSource
    {
        Vector<double> _RRInterval;
        private double _binLength;
        private ObservableCollection<Sample> _samples;

        public DataSource(double binLength, List<Tuple<string, Vector<double>>> RRInterval ) //chialam wziac dane z R_Peaks ale nie potrafie
        {
            Tuple<string, Vector<double>> RRIntervalTuple = RRInterval.First();
            _RRInterval = RRIntervalTuple.Item2;
            _binLength = binLength;
            _samples = new ObservableCollection<Sample>();
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
            if (_samples.Count == 0)
                getSamplesData();

            return;
        }

        private void getSamplesData()
        {
            if (_samples.Count != 0)
                return;
        }

        private void groupSamples(List<Double> samples)
        {
            double start = samples[0];
            List<List<Double>> _helperListOfSampleList = new List<List<Double>>();
            List<Double> _helperSampleList = new List<Double>();
            foreach (Double d in samples)
            {
                _helperSampleList.Add(d);
                if (d >= start + _binLength)
                {
                    _helperListOfSampleList.Add(_helperSampleList);
                    _helperSampleList = new List<Double>();
                    start = d;
                }
            }
            try
            {
                checkCountOfSamples(_helperListOfSampleList, samples);
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
                throw new System.Exception("Klops");
            }
       }

        #endregion
    }
}
