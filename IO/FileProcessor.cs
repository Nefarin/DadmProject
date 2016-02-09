using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    public class FileProcessor
    {
        private enum State {INIT, NEXT, BEGIN_NEXT, PROCESS, END}
        private int _numberOfLeads;
        private int _currentLeadIndex;
        private int _currentIndex;
        private int _step;
        private int _currentLeadLength;
        private List<string> _leads;
        private string _analysisName;
        private IECGConverter _converter;
        private State _state;
        private bool _ended;
        private Basic_New_Data_Worker _worker;

        private int Step
        {
            get
            {
                return _step;
            }

            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException();
                _step = value;
            }
        }

        private IECGConverter Converter
        {
            get
            {
                return _converter;
            }

            set
            {
                if (value == null) throw new ArgumentNullException();
                _converter = value;
            }
        }

        public FileProcessor(IECGConverter converter, string analysisName, int step)
        {
            Converter = converter;
            _analysisName = analysisName;
            _state = State.INIT;
            _leads = Converter.getLeads();
            _numberOfLeads = _leads.Count;
            _ended = false;
            Step = step;
            _worker = new Basic_New_Data_Worker(_analysisName);
        }

        public bool Ended()
        {
            return _ended;
        }
        public void Process()
        {
            switch (_state)
            {
                case (State.INIT):
                    _currentLeadIndex = -1;
                    _currentIndex = 0;
                    _state = State.NEXT;
                    Converter.DeleteFiles();
                    break;
                case (State.NEXT):
                    _currentLeadIndex++;
                    _currentIndex = 0;
                    if (_currentLeadIndex < _numberOfLeads)
                    {
                        _currentLeadLength = (int)Converter.getNumberOfSamples(_leads[_currentLeadIndex]);
                        _state = State.BEGIN_NEXT;
                        break;
                    }
                    else _state = State.END;   
                    break;
                case (State.BEGIN_NEXT):
                    try
                    {
                        Vector<double> vect = Converter.getSignal(_leads[_currentLeadIndex], _currentIndex, Step);
                        _worker.SaveSignal(_leads[_currentLeadIndex], false, vect);
                        _currentIndex += Step;
                        _state = State.PROCESS;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            Vector<double> vect = Converter.getSignal(_leads[_currentLeadIndex], _currentIndex, _currentLeadLength - _currentIndex);
                            _worker.SaveSignal(_leads[_currentLeadIndex], false, vect);
                            _currentIndex += _currentLeadLength - _currentIndex;
                            _state = State.NEXT;
                        }
                        catch (OverflowException k)
                        {
                            _state = State.NEXT;
                        }

                    }
                    break;
                case (State.PROCESS):
                    try
                    {
                        Vector<double> vect = Converter.getSignal(_leads[_currentLeadIndex], _currentIndex, Step);
                        _worker.SaveSignal(_leads[_currentLeadIndex], true, vect);
                        _state = State.PROCESS;
                        _currentIndex += Step;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            Vector<double> vect = Converter.getSignal(_leads[_currentLeadIndex], _currentIndex, _currentLeadLength - _currentIndex);
                            _worker.SaveSignal(_leads[_currentLeadIndex], true, vect);
                            _state = State.NEXT;
                            _currentIndex += _currentLeadLength - _currentIndex;
                        }
                        catch (OverflowException k)
                        {
                            _state = State.NEXT;
                        }

                    }
                    break;
                case (State.END):
                    _worker.SaveLeads(_leads);
                    _worker.SaveAttribute(Basic_Attributes.Frequency, Converter.getFrequency());
                    _ended = true;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}
