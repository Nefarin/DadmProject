using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    public class Processing
    {
        private int _progressStatus;

        private ProcessSync _communication;
        private AnalysisState _command;
        private AnalysisState _timeoutCommand;
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="communication"></param>
        /// 
        #endregion
        public Processing(ProcessSync communication)
        {
            _progressStatus = 0;
            _communication = communication;
            _timeoutCommand = AnalysisState.IDLE;
        }
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public void run()
        {
            Boolean run = true;
            while (run)
            {
                var item =_communication.getGUIMessage(20);
                if (item.Command == AnalysisState.TIMEOUT)
                {
                    _command = _timeoutCommand;
                }
                else
                {
                    _command = item.Command;
                }

                switch (_command)
                {
                    case (AnalysisState.IDLE):
                        Thread.Sleep(5);
                        break;
                    case (AnalysisState.INIT):
                        _command = AnalysisState.IDLE;
                        break;
                    case (AnalysisState.STOP_ANALYSIS):
                        run = false;
                        _communication.SendProcessingEvent(new ToGUIItem(ToGUICommand.EXIT_ANALYSIS, null));
                        break;
                    case (AnalysisState.ADD_TEST):
                        _progressStatus += 20;
                        _communication.SendProcessingEvent(new ToGUIItem(ToGUICommand.TEST, _progressStatus));
                        break;
                    case (AnalysisState.SUB_TEST):
                        _progressStatus -= 20;
                        _communication.SendProcessingEvent(new ToGUIItem(ToGUICommand.TEST, _progressStatus));
                        break;
                    default:
                        _command = AnalysisState.STOP_ANALYSIS;
                        break;
                }

            }
        }

    }
}
