﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    class TempInput
    {
        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        /// <param name="pathIn"></param>
        /// <param name="pathOut"></param>
        /// 
        #endregion

        static string pathIn;
        static string pathOut;

        static void setInputFilePath(string s)
        {
             pathIn = s;
        }

        static void setOutputFilePath(string s)
        {
            pathOut = s;
        }

        static uint getFrequency()
        {
            uint frequency = 0;
            using (StreamReader sr = new StreamReader(pathIn)) 
            {
                string fileData = sr.ReadToEnd();
                string[] fileLines = fileData.Split('\n');
                frequency = Convert.ToUInt32(fileLines[0]);
           }
            return frequency;
        }

        static Vector<double> getSignal()
        {
            string sig = "";
            using (StreamReader sr = new StreamReader(pathIn))
            {
                string fileData = sr.ReadToEnd();
                string[] fileLines = fileData.Split('\n');
                sig = fileLines[1];
            }
            Vector<double> signal = stringToVector(sig);
            return signal;
        }

        static void writeFile(uint frequency, Vector<double> signal)
        {
            frequency.ToString();
            string input = vectorToString(signal);

            using (StreamWriter sw = new StreamWriter(pathOut))
            {
                sw.WriteLine(frequency);
                sw.Write(input);
            }

        }

        static Vector<double> stringToVector(string input)
        {
            double[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => double.Parse(digit))
                              .ToArray();
            Vector<double> vector = Vector<double>.Build.Dense(digits.Length);
            vector.SetValues(digits);
            return vector;
                       
        }

        static string vectorToString(Vector<double> vector)
        {
            string output = "";
            for (int i = 0; i < vector.Count; i++)
            {
                output += vector[i] + " ";
            }
            return output;
        }
        
        /*
        static void Main()
        {
            setInputFilePath(@"C:\temp\in.txt");
            uint fs = getFrequency();
            Vector<double> sig = getSignal();

            setOutputFilePath(@"C:\temp\out.txt");
            writeFile(fs, sig);
        }*/
    }
}