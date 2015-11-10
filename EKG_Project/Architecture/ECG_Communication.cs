using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace EKG_Project.Architecture
{
    #region Event Delegate
    public delegate void Analysis_To_GUI(object sender, Analysis_To_GUI_Item item);
    #endregion

    #region enumDeclaration GUI_To_Analysis
    public enum GUI_To_Analysis_Command {INIT, IDLE, STOP_ANALYSIS, TIMEOUT };
    #endregion

    #region enumDeclaration Analysis_To_GUI
    public enum Analysis_To_GUI_Command { ANALYSIS_ENDED, EXIT_ANALYSIS };
    #endregion

    #region GUI_To_Analysis_Item
    public class GUI_To_Analysis_Item
    {
        private GUI_To_Analysis_Command _command;
        private object _data;

        public GUI_To_Analysis_Item(GUI_To_Analysis_Command command, object data)
        {
            _command = command;
            _data = data;
        }

        #region Properties
        public object Data
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

        internal GUI_To_Analysis_Command Command
        {
            get
            {
                return _command;
            }

            set
            {
                _command = value;
            }
        }
        #endregion
    }
    #endregion

    #region Analysis_To_GUI_Item
    public class Analysis_To_GUI_Item
    {
        private Analysis_To_GUI_Command _command;
        private object _data;

        public Analysis_To_GUI_Item(Analysis_To_GUI_Command command, object data)
        {
            _command = command;
            _data = data;
        }

        #region Properties
        public Analysis_To_GUI_Command Command
        {
            get
            {
                return _command;
            }

            set
            {
                _command = value;
            }
        }

        public object Data
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
        #endregion
    }
    #endregion

    public class ECG_Communication
    {
        public event Analysis_To_GUI Analysis_To_GUI_Event;

        private Queue<GUI_To_Analysis_Item> _guiToAnalysis;

        public ECG_Communication()
        {
            _guiToAnalysis = new Queue<GUI_To_Analysis_Item>();
        }

        public void sendGUIMessage(GUI_To_Analysis_Item item)
        {
            _guiToAnalysis.Enqueue(item);
        }
        public GUI_To_Analysis_Item getGUIMessage(int timeout = 0)
        {
            int counter = -1;
            while (counter <= timeout)
            {
                if(_guiToAnalysis.Count != 0)
                {
                    return _guiToAnalysis.Dequeue();
                }
                else
                {
                    counter += 5;
                    Thread.Sleep(5);
                }
            }

            return new GUI_To_Analysis_Item(GUI_To_Analysis_Command.TIMEOUT, null);
        }

        public void sendAnalysisEvent(Analysis_To_GUI_Item item)
        {
            Analysis_To_GUI_Event(this, item);
        }


    }
}
