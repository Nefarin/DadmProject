using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    /// <summary>
    /// Temporary class to read txt files
    /// </summary>
    class TempInput
    {
        static string pathIn;
        static string pathOut;

        public static void setInputFilePath(string s)
        {
             pathIn = s;
        }

        public static void setOutputFilePath(string s)
        {
            pathOut = s;
        }

        public static uint getFrequency()
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

        public static Vector<double> getSignal()
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

        public static void writeFile(uint frequency, Vector<double> signal)
        {
            frequency.ToString();
            string input = vectorToString(signal);

            using (StreamWriter sw = new StreamWriter(pathOut))
            {
                sw.WriteLine(frequency);
                sw.Write(input);
            }
        }

        public static void writeFile(List <Tuple <double, double>> output)
        {
            output.ToString();

            using (StreamWriter sw = new StreamWriter(pathOut))
            {
                sw.WriteLine(output);
            }
        }


        public static void writeFile(uint frequency, Vector<double> signalX, Vector<double> signalY)
        {
            frequency.ToString();
            string inputX = vectorToString(signalX);
            string inputY = vectorToString(signalY);

            using (StreamWriter sw = new StreamWriter(pathOut))
            {
                sw.WriteLine(frequency);
                sw.WriteLine(inputX);
                sw.Write(inputY);
            }
        }

        public static Vector<double> stringToVector(string input)
        {
            double[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => Convert.ToDouble(digit, new System.Globalization.NumberFormatInfo()))
                              .ToArray();
            Vector<double> vector = Vector<double>.Build.Dense(digits.Length);
            vector.SetValues(digits);
            return vector;
                       
        }

        public static string vectorToString(Vector<double> vector)
        {
            string output = "";
            for (int i = 0; i < vector.Count; i++)
            {
                output += vector[i] + " ";
            }
            return output;
        }
        
        
        static void Main()
        {
            setInputFilePath(@"C:\temp\RR_100.txt");
            uint fs = getFrequency();
            Vector<double> sig = getSignal();
            Console.Write(sig);
            Console.Read();

            //setOutputFilePath(@"C:\temp\out.txt");
            //writeFile(fs, sig);
        }
    }
}
