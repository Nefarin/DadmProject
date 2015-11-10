using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EKG_Project.Architecture
{
    public class ECG_Analysis
    {
        private int _progressStatus;

        private ECG_Communication _communication;
        private GUI_To_Analysis_Command _command;
        private GUI_To_Analysis_Command _timeoutCommand;

        public ECG_Analysis(ECG_Communication communication)
        {
            _progressStatus = 0;
            _communication = communication;
            _timeoutCommand = GUI_To_Analysis_Command.IDLE;
        }
        public void run()
        {
            Boolean run = true;
            while (run)
            {
                var item =_communication.getGUIMessage(20);
                if (item.Command == GUI_To_Analysis_Command.TIMEOUT)
                {
                    _command = _timeoutCommand;
                }
                else
                {
                    _command = item.Command;
                }

                switch (_command)
                {
                    case (GUI_To_Analysis_Command.IDLE):
                        Thread.Sleep(5);
                        break;
                    case (GUI_To_Analysis_Command.INIT):
                        _command = GUI_To_Analysis_Command.IDLE;
                        break;
                    case (GUI_To_Analysis_Command.STOP_ANALYSIS):
                        run = false;
                        _communication.sendAnalysisEvent(new Analysis_To_GUI_Item(Analysis_To_GUI_Command.EXIT_ANALYSIS, null));
                        break;
                    case (GUI_To_Analysis_Command.ADD_TEST):
                        _progressStatus += 20;
                        _communication.sendAnalysisEvent(new Analysis_To_GUI_Item(Analysis_To_GUI_Command.TEST, _progressStatus));
                        break;
                    case (GUI_To_Analysis_Command.SUB_TEST):
                        _progressStatus -= 20;
                        _communication.sendAnalysisEvent(new Analysis_To_GUI_Item(Analysis_To_GUI_Command.TEST, _progressStatus));
                        break;
                    default:
                        _command = GUI_To_Analysis_Command.STOP_ANALYSIS;
                        break;
                }

            }
        }

    }
}
