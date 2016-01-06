using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;

namespace EKG_Project.Modules.Waves
{
    /*
    public partial class Waves : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker;

        private Basic_Data _inputData;
        private Waves_Params _params;

        private Waves_Data _data;

        private Vector<Double> _currentVector;

        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters, ECG_Data data)
        {
            Params = parameters as Waves_Params;
            Data = data as Waves_Data;
            
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;
            }
        }

        public void Init(ModuleParams parameters)
        {
            Params = parameters as Waves_Params;

            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;
            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        public Basic_Data_Worker InputWorker
        {
            get
            {
                return _inputWorker;
            }

            set
            {
                _inputWorker = value;
            }
        }

        private void processData()
        {
            DetectQRS();
            FindP();
            FindT();
            //tu zara sie cos dorzuci
        }

        public Basic_Data InputData
        {
            get
            {
                return _inputData;
            }

            set
            {
                _inputData = value;
            }
        }

        public int NumberOfChannels
        {
            get
            {
                return _numberOfChannels;
            }

            set
            {
                _numberOfChannels = value;
            }
        }

        public bool Aborted
        {
            get
            {
                return _aborted;
            }

            set
            {
                _aborted = value;
            }
        }

        public Waves_Params Params
        {
            get
            {
                return _params;
            }

            set
            {
                _params = value;
            }
        }

        public Waves_Data Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public static void Main()
        {
            
            //POKI CO BIERZEMY DANE Z NASZYCH GOWNIANYCH PLIKOW
        
            TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG.txt");
            TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKGQRSonsets3.txt");
            /*TempInput.setInputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKG.txt");
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGQRSonsets.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> ecg = TempInput.getSignal();
            //Vector<double> dwt = ListDWT(_ecg, 3, Wavelet_Type.db2)[1];
            Vector<double> temp = Vector<double>.Build.Dense(2);

            TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG3Rpeaks.txt");
            //TempInput.setInputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKG3Rpeaks.txt");

            List<int> Rpeaks = new List<int>();
            Vector<double> rpeaks = TempInput.getSignal();
            foreach (double singlePeak in rpeaks)
            {
                Rpeaks.Add((int)singlePeak);
            }


            Waves_Params param = new Waves_Params(Wavelet_Type.haar , 2 , "Analysis6");
            Waves_Data data = new Waves_Data(ecg, Rpeaks, fs);


            Waves testModule = new Waves();

            testModule.Init(param, data);
            testModule.ProcessData();
            data = testModule.Data;

            Vector<double> onsets = Vector<double>.Build.Dense(data.QRSOnsets.Count);
            for (int i = 0; i < data.QRSOnsets.Count; i++)
            {
                onsets[i] = (double)data.QRSOnsets[i];

            }

            TempInput.writeFile(360, onsets);
            //Vector<double> ends = Vector<double>.Build.Dense(_QRSends.Count);
            //for (int i = 0; i < _QRSends.Count; i++)
            //{
            //    ends[i] = (double)_QRSends[i];

            //}
            //FindP();
            //Vector<double> ponset = Vector<double>.Build.Dense(_Ponsets.Count);
            //for (int i = 0; i < _Ponsets.Count; i++)
            //{
            //    ponset[i] = (double)_Ponsets[i];

            //}
            //Vector<double> pends = Vector<double>.Build.Dense(_Pends.Count);
            //for (int i = 0; i < _Pends.Count; i++)
            //{
            //    pends[i] = (double)_Pends[i];

            //}
            //FindT();
            //Vector<double> tends = Vector<double>.Build.Dense(_Tends.Count);
            //for (int i = 0; i < _Tends.Count; i++)
            //{
            //    tends[i] = (double)_Tends[i];

            //}

            //TempInput.writeFile(360, onsets);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGQRSends.txt");
            //TempInput.writeFile(360, ends);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGPonsets.txt");
            //TempInput.writeFile(360, ponset);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGPends.txt");
            //TempInput.writeFile(360, pends);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGTends.txt");
            //TempInput.writeFile(360, tends);
            //TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\d2ekg.txt");
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\d2ekg.txt");
            //TempInput.writeFile(360, dwt);
            Console.Read();
        }

    }
     * */
}
