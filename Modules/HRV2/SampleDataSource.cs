using Histogram.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

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

    public class SampleDataSource
    {
        private string FILENAME;
        private double _binLength;
        private ObservableCollection<Sample> _samples;

        public SampleDataSource(double binLength, string fileName = null)
        {
            this.FILENAME = fileName;
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

            try
            {
                string[] lines = System.IO.File.ReadAllLines(FILENAME);
                retrieveSamples(lines);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void retrieveSamples(string[] table)
        {
            try
            {
                List<Double> samples;
                Converters.StringArrayToDoubleList(table, out samples);
                samples.Sort();
                groupSamples(samples);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
                throw new System.Exception("Zadzwoń do mnie, coś jest nie tak");
            }
       }

        #endregion
    }
}
