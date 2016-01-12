using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;

namespace EKG_Project.Modules.Heart_Class
{
    public partial class Heart_Class : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private int _channel2;
        private int startIndex;
        private bool _ml2Processed;
        private int _numberOfSteps;

        // Czy te zmienne muszą być private? 
        int qrsEndStep;// o tyle qrs się przesuwamy 
        int i; //do inkrementacji co 10
        int step; // ilość próbek o którą sie przesuwamy

        private ECG_Baseline_Data_Worker _inputECGbaselineWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Waves_Data_Worker _inputWavesWorker;
        private Heart_Class_Data_Worker _outputWorker;
        private Basic_Data_Worker _basicDataWorker;

        private ECG_Baseline_Data _inputECGbaselineData;
        private R_Peaks_Data _inputRpeaksData;
        private Waves_Data _inputWavesData;
        private Heart_Class_Data _outputData;
        private Basic_Data _basicData;

        private Heart_Class_Params _params;

        private Vector<Double> _currentVector;
        private Tuple<int, int> _tempClassResult;



        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            
            Params = parameters as Heart_Class_Params;
            

            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputEcGbaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputEcGbaselineWorker.Load();
                InputECGbaselineData = InputEcGbaselineWorker.Data;
                OutputData = new Heart_Class_Data();

                if (findChannel())
                {
                    InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                    InputRpeaksWorker.Load();
                    InputRpeaksData = InputRpeaksWorker.Data;

                    BasicDataWorker = new Basic_Data_Worker(Params.AnalysisName);
                    BasicDataWorker.Load();
                    BasicData = BasicDataWorker.BasicData;
                    fs = BasicData.Frequency;

                    InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
                    InputWavesWorker.Load();
                    InputWavesData = InputWavesWorker.Data;
                    _currentRVector = InputRpeaksData.RPeaks[_channel2].Item2;
                    _currentECGBaselineVector = InputECGbaselineData.SignalsFiltered[_channel2].Item2;
                   
                    OutputWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
              
                    _currentChannelIndex = 0;
                    _samplesProcessed = 0;
                    startIndex = _samplesProcessed;
                    NumberOfChannels = InputECGbaselineData.SignalsFiltered.Count;
                    _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                    _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    qrsEndStep = 10;
                    i = 10;
                    _numberOfSteps = InputWavesData.QRSEnds[_channel2].Item2.Count;
                    step = 1;
                    //ilośc próbek, aż do indeksu końca 10 załamka
                    _tempClassResult = new Tuple<int, int>(0,0);
                    _ml2Processed = false;

                }
                else
                {
                    _ended = true;
                    _aborted = true;
                }
            }
            
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }
        
        public double Progress()
        {

            return 100.0*_samplesProcessed/_numberOfSteps;
        }
        
        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {

            if (!_ml2Processed)
            {

                int QRSOnSet = InputWavesData.QRSOnsets[_channel2].Item2[_samplesProcessed];
                int QRSEnds = InputWavesData.QRSEnds[_channel2].Item2[_samplesProcessed];
                double R = InputRpeaksData.RPeaks[_channel2].Item2[_samplesProcessed];

                if (QRSOnSet == -1 || QRSEnds == -1)
                {
                    _samplesProcessed++;
                }
                else
                {
                    _tempClassResult = ClassificationOneQrs(InputECGbaselineData.SignalsFiltered[_channel2].Item2,
                                        QRSOnSet, QRSEnds, R);
                    OutputData.ClassificationResult.Add(_tempClassResult);

                    _samplesProcessed++;
                }

                if (_samplesProcessed >= _numberOfSteps)
                {
                    _ml2Processed = true;
                }
            }
            else
            {
                _ended = true;
                OutputWorker.Save(OutputData);
            }
            /*
            int channel = _currentChannelIndex;
            int startIndex = _samplesProcessed;
            //int qrsEndStep = 10;// o tyle qrs się przesuwamy
            //int i = 10; //do inkrementacji co 10
            //int step; // ilość próbek o którą sie przesuwamy

            if (channel < NumberOfChannels)
            {
                //step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep];

                Console.WriteLine(step);
                Console.WriteLine(startIndex);
                Console.WriteLine(_currentChannelLength);
                if (startIndex + step > _currentChannelLength)
                {
                    

                    Console.WriteLine("jestem tuuu");
                    _currentVector = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex);

                    //OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(Classification(_currentVector, fs, InputRpeaksData.RPeaks[_currentChannelIndex].Item2, InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.QRSEnds[_currentChannelIndex].Item2)));
                    _tempClassResult = Classification(_currentVector, fs,
                        InputRpeaksData.RPeaks[_currentChannelIndex].Item2,
                        InputWavesData.QRSOnsets[_currentChannelIndex].Item2,
                        InputWavesData.QRSEnds[_currentChannelIndex].Item2);
                    OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(_tempClassResult));
                    _currentChannelIndex++;

                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                        //_tempClassResult = new List<Tuple<int, int>>(); //kasowanie tego co było wczesniej zapisane, czy tak moze byc?
                    }

                   
                }
                else
                {
                    
                    _currentVector = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, step);

                    _tempClassResult = Classification(_currentVector, fs,
                        InputRpeaksData.RPeaks[_currentChannelIndex].Item2,
                        InputWavesData.QRSOnsets[_currentChannelIndex].Item2,
                        InputWavesData.QRSEnds[_currentChannelIndex].Item2);
                    OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(_tempClassResult));

                    _samplesProcessed = startIndex + step;
                   
                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }

            qrsEndStep += i;
            step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep] - step;
            */


        }

        private bool findChannel()
        {
            int i = 0;
            
            foreach (var tuple in InputECGbaselineData.SignalsFiltered)
            {
                string name = tuple.Item1;
                if (name == "MLII" || name == "II")
                {
                    Channel2 = i;
                    OutputData.ChannelMliiDetected = true;
                    return true;
                }
                i++;
            }
            OutputData.ChannelMliiDetected = false;
            return false;
        }

        public Heart_Class_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }
        
        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public Heart_Class_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }
       
        public Heart_Class_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }
        


        public ECG_Baseline_Data_Worker InputEcGbaselineWorker
        {
            get { return _inputECGbaselineWorker; }
            set { _inputECGbaselineWorker = value; }
        }

        public R_Peaks_Data_Worker InputRpeaksWorker
        {
            get { return _inputRpeaksWorker; }
            set { _inputRpeaksWorker = value; }
        }

        public Waves_Data_Worker InputWavesWorker
        {
            get { return _inputWavesWorker; }
            set { _inputWavesWorker = value; }
        }

        public ECG_Baseline_Data InputECGbaselineData
        {
            get { return _inputECGbaselineData; }
            set { _inputECGbaselineData = value; }
        }

        public R_Peaks_Data InputRpeaksData
        {
            get { return _inputRpeaksData; }
            set { _inputRpeaksData = value; }
        }

        public Waves_Data InputWavesData
        {
            get { return _inputWavesData; }
            set { _inputWavesData = value; }
        }

        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }

        public int Channel2
        {
            get { return _channel2; }
            set { _channel2 = value; }
        }

        public Basic_Data_Worker BasicDataWorker
        {
            get { return _basicDataWorker; }
            set { _basicDataWorker = value; }
        }

        public Basic_Data BasicData
        {
            get { return _basicData; }
            set { _basicData = value; }
        }


        public static void Main()
        {
            Heart_Class_Params param = new Heart_Class_Params("TestAnalysis2"); // "Analysis6");
            Heart_Class testModule = new Heart_Class();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }




            /*
            Heart_Class HeartClass = new Heart_Class();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\signal.txt");
            uint fs = TempInput.getFrequency();
            HeartClass.Signal = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsOnset.txt");
            //ponizej chce wczytac jako wektor a to jest juz lista
            //HeartClass.QrsOnset = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            //HeartClass.QrsEnd = TempInput.getSignal();

            // uwaga tu mam pozniej wrzucic plik qrsR.txt !!!!
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            HeartClass.QrsR = TempInput.getSignal();

            
            //WCZYTANIE ZESPOŁÓW QRS NA PODSTAWIE QRSonsets i QRSends
            HeartClass.SetQrsComplex(); 

            //LICZENIE WSPÓŁCZYNNIKÓW KSZTAŁTU
            HeartClass.QrsCoefficients = HeartClass.CountCoeff(HeartClass.GetQrsComplex(), fs);

            
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, HeartClass.Signal);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, HeartClass.QrsOnset);

            //WCZYTANIE ZBIORU TRENINGOWEGO
            List<Vector<double>> trainDataList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d.txt");


            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            List<Vector<double>> trainClassList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d_label.txt");
            // konwersja na listę intów, bo tak napisałam metodę do klasyfikacji:
                    int oneClassElement;
                    List<int> trainClass;
                    trainClass = new List<int>();
                    foreach (var item in trainClassList)
                    {
                        foreach (var element in item)
                        {
                            oneClassElement = (int)element;
                            trainClass.Add(oneClassElement);
                        }

                    }


            List<Vector<double>> testDataList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\test_d.txt");
            // Tworzenie listy tupli zbioru testowego - w celach testowych (zbior treningowy i testowy wczytywany jest z pliku). 
            //w ostatecznej  wresji testDataList będzie obliczane w programie w formie:  List<Tuple<int, Vector<double>>>: 
                    List<Tuple<int, Vector<double>>> testSamples;
                    testSamples = new List<Tuple<int, Vector<double>>>();
                    Tuple<int, Vector<double>> oneElement;
                    int R = 1; 
                    foreach (var item in testDataList)
                    {
                        oneElement = new Tuple<int, Vector<double>>(R, item.Clone());
                        testSamples.Add(oneElement);
                    }

            //KLASYFIKACJA
            HeartClass.HeartClassData.ClassificationResult = HeartClass.TestKnnCase(trainDataList, HeartClass.QrsCoefficients, trainClass, 1); // klasyfikacja sygnału testowego signal
            //HeartClass.classificationResult = HeartClass.TestKnnCase(trainDataList, testSamples, trainClass, 1); // jeśli chcemy prztestować zbiór testowy (z matlaba)

        */

            // nie działa bo onset i end są już List<int>
            //HeartClass.HeartClassData.ClassificationResult = HeartClass.Classification(HeartClass.Signal, fs,
            //    HeartClass.QrsR, HeartClass.QrsOnset, HeartClass.QrsEnd);
        }
    }
}
