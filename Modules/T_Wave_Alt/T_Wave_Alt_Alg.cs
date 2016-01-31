using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Alg
    {
        // Class fields
        private T_Wave_Alt_Params _params;
        private uint _fs;
        private int _tLength;

        // Get&set
        public T_Wave_Alt_Params Params
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
        public uint Fs
        {
            get
            {
                return _fs;
            }

            set
            {
                _fs = value;
            }
        }
        public int TLength
        {
            get
            {
                return _tLength;
            }

            set
            {
                _tLength = value;
            }
        }

        // Class methods

        #region Documentation
        /// <summary>
        /// This function calculates the length of an average T-wave in samples, used by many further functions.
        /// </summary>
        /// <param name="fs">Sampling frequency</param>
        /// <returns>Average T-length in samples</returns>
        #endregion
        private void calculateTLength()
        {
            double t_length1 = this.Fs * 0.15;
            this.TLength = Convert.ToInt32(t_length1);
        }

        #region Documentation
        /// <summary>
        /// This function checks if all of the medians are equal to 0; if so, it changes the median to standard value.
        /// </summary>
        /// <param name="medianVector">Median T-wave vector</param>
        /// <returns>True if all of the vectors elements are 0, false otherwise.</returns>
        #endregion
        private bool medianZeroCheck(Vector<double> medianVector)
        {
            bool isZero = true;
            foreach (double element in medianVector)
            {
                if (element != 0)
                {
                    isZero = false;
                    break;
                }
            }

            return isZero;
        }

        #region Documentation
        /// <summary>
        /// This function extracts T-waves based on the input signal and indices of T-ends.
        /// </summary>
        /// <param name="loadedSignal">Input signal</param>
        /// <param name="tEndsList">List of T-ends indices</param>
        /// <returns>List of vectors containing T-waves</returns>
        #endregion
        public List<Vector<double>> buildTWavesArray(Vector<double> loadedSignal, List<int> tEndsList)
        {
            calculateTLength();
            int tLength = this.TLength;
            List<Vector<double>> TWavesArray = new List<Vector<double>>();
            foreach (int currentTEnd in tEndsList) {
                if (currentTEnd + 1 >= tLength && currentTEnd >= 0 && currentTEnd < loadedSignal.Count)
                {
                    Vector<double> newTWave = loadedSignal.SubVector(currentTEnd - tLength + 1, tLength);
                    TWavesArray.Add(newTWave);
                }
            }

            return TWavesArray;
        }

        #region Documentation
        /// <summary>
        /// This function calculates the medians of corresponding samples in T-waves vectors
        /// </summary>
        /// <param name="TWavesArray">List of vectors containing T-waves</param>
        /// <returns>Vector containing median T-wave</returns>
        #endregion
        public Vector<double> calculateMedianTWave(List<Vector<double>> TWavesArray)
        {
            int tLength = this.TLength;

            Vector<double> medianVector = Vector<double>.Build.Dense(tLength);
            Vector<double> tempColumn = Vector<double>.Build.Dense(TWavesArray.Count);
            for (int column = 0; column < tLength; column++)
            {
                for (int row = 0; row < TWavesArray.Count(); row++)
                {
                    Vector<double> currentTWave = TWavesArray[row];
                    tempColumn[row] = currentTWave[column];
                }
                double[] tempColumn2 = tempColumn.ToArray();
                Array.Sort(tempColumn2);
                if (TWavesArray.Count % 2 == 0) medianVector[column] = (tempColumn2[TWavesArray.Count / 2] + tempColumn2[(TWavesArray.Count / 2) - 1]) / 2;
                else medianVector[column] = tempColumn2[(TWavesArray.Count-1)/2];
            }

            return medianVector;
        }
   
        #region Documentation
        /// <summary>
        /// This function calculates ACI values for all of the detected T-waves
        /// </summary>
        /// <param name="TWavesArray">List of vectors containing T-waves</param>
        /// <param name="medianVector">Vector containing median T-wave</param>
        /// <returns>Vector containing ACI values</returns>
        #endregion
        public Vector<double> calculateACI(List<Vector<double>> TWavesArray, Vector<double> medianVector)
        {
            Vector<double> ACIVector = Vector<double>.Build.Dense(TWavesArray.Count);

            if (medianZeroCheck(medianVector)) medianVector = medianVector + 1;

            int WaveIndex = 0;
            foreach (Vector<double> currentTWave in TWavesArray)
            {
                double sumNom = 0;
                double sumDenom = 0;

                int count = 0;
                foreach (double singleSample in currentTWave)
                {
                    double ACI_auxNom = singleSample * medianVector[count];
                    double ACI_auxDenom = medianVector[count] * medianVector[count];
                    sumNom = sumNom + ACI_auxNom;
                    sumDenom = sumDenom + ACI_auxDenom;
                    count++;
                }
               
                ACIVector[WaveIndex] = sumNom/sumDenom;
                WaveIndex++;
            }

            return ACIVector;
        }

        #region Documentation
        /// <summary>
        /// This function finds ACI fluctuations around 1
        /// </summary>
        /// <param name="ACIVector">Vector containing ACI values</param>
        /// <returns> List containing 1s when fluctuation had occured, 0s when it hadn't</returns>
        #endregion
        public List<int> findFluctuations(Vector<double> ACIVector)
        {
            List<int> fluctuationsList= new List<int>(ACIVector.Count);
            fluctuationsList.Add(0);

            for (int i = 1; i < ACIVector.Count; i++)
            {
                if ((ACIVector[i - 1] <= 1 && ACIVector[i] > 1) || (ACIVector[i - 1] >= 1 && ACIVector[i] < 1))
                {
                    fluctuationsList.Add(1);
                }
                else fluctuationsList.Add(0);
            }

            return fluctuationsList;
        }

        #region Documentation
        /// <summary>
        /// This function detects T-wave alternans, based on the frequency of the fluctuations of the ACI values.
        /// </summary>
        /// <param name="fluctuationsList">List containing 1s when fluctuation had occured, 0s when it hadn't</param>
        /// <returns> Vector containing 1s where alternans is detected</returns>
        #endregion
        public Vector<double> findAlternans(List<int> fluctuationsList)
        {
            Vector<double> alternansVector = Vector<double>.Build.Dense(fluctuationsList.Count);
            alternansVector = alternansVector + 1;
            int alterThreshold = 3;
            int firstElement = 0;
            int counter = 0;
            int forEachIndex = 0;

            foreach (int element in fluctuationsList)
            {
                if (counter == 0)
                {
                    if (element == 0)
                    {
                        firstElement = forEachIndex;
                        counter++;
                    }
                }

                else if ((counter > 0) && (counter < alterThreshold))
                {
                    if (element == 0) counter++;
                    else counter = 0;
                }

                else if (counter >= alterThreshold)
                {
                    if (element == 0)
                    {
                        if (forEachIndex + 1 != fluctuationsList.Count) counter++;
                        else
                        {
                            for (int p = firstElement; p <= firstElement+counter; p++)
                            {
                                alternansVector[p] = 0;
                            }
                            counter = 0;
                            firstElement = 0;
                        }
                    }

                    else
                    {
                        for (int p = firstElement; p < firstElement+counter; p++)
                        {
                            alternansVector[p] = 0;
                        }
                        counter = 0;
                        firstElement = 0;
                    }
                }
                forEachIndex++;
            }

            return alternansVector;
        }

        #region Documentation
        /// <summary>
        /// This function produces final ouput data in a proper format (for visualisation)
        /// </summary>
        /// <param name="alternansVector">Vector containing 1s where alternans is detected</param>
        /// <param name="tEndsList">List of T-ends indices</param>
        /// <returns>List of tuples. Each tuple consists of a heartbeat index and a '1' value for confirmation</returns>
        #endregion
        public List<Tuple<int, int>> alternansDetection(Vector<double> alternansVector, List<int> tEndsList)
        {
            List<Tuple<int, int>> alternansDetectedList = new List<Tuple<int, int>>();

            for (int i = 0; i < alternansVector.Count; i++)
            {
                if (alternansVector[i]==1)
                {
                    Tuple<int, int> currentRecord = new Tuple<int, int>(tEndsList[i], 1);
                    alternansDetectedList.Add(currentRecord);
                }
            }

            return alternansDetectedList;
        } 
       
    }
    
}
