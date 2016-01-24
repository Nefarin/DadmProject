using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EKG_Project.Architecture.ProcessingStates;
using EKG_Project.Modules;

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
        private Modules _modules;
        private Stats _stats;
        private ProcessSync _communication;
        private IProcessingState _timeoutState;
        private bool _stop;

        #region Properties
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public Modules Modules
        {
            set
            {
                _modules = value;
            }
            get
            {
                return _modules;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public ProcessSync Communication
        {
            get
            {
                return _communication;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public IProcessingState TimeoutState
        {
            get
            {
                return _timeoutState;
            }

            set
            {
                _timeoutState = value;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public bool Stop
        {
            get
            {
                return _stop;
            }
            set
            {
                _stop = value;
            }
        }

        public Stats Stats
        {
            get
            {
                return _stats;
            }

            set
            {
                _stats = value;
            }
        }

        #endregion

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="communication"></param>
        /// 
        #endregion
        public Processing(ProcessSync communication, String analysisName)
        {
            this._communication = communication;
            Modules = new Modules(analysisName);
            Stats = new Stats(analysisName);
            TimeoutState = new Idle(5);
            Stop = false;
        }

        public Processing(bool _stop)
        {
            this._stop = _stop;
        }
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public void run()
        {
            while (!this.Stop)
            {
                IProcessingState state;
                var timeouted =_communication.GetGUIMessage(out state, 20);
                if (timeouted)
                {
                    TimeoutState.Process(this, out _timeoutState);
                }
                else
                {
                    state.Process(this, out _timeoutState);
                }
            }
        }

    }
}
