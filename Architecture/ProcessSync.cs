using System.Collections.Generic;
using System.Threading;
using EKG_Project.Architecture.ProcessingStates;
using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// Class responsible for GUI-Analysis thread synchoronization.
    /// </summary>
    #endregion
    public class ProcessSync
    {
        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// 
        #endregion
        public event ToGUIDelegate ToGUIEvent;

        private Queue<IProcessingState> _toProcessingQueue;

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public ProcessSync()
        {
            _toProcessingQueue = new Queue<IProcessingState>();
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="state"></param>
        /// 
        #endregion
        public void SendGUIMessage(IProcessingState state)
        {
            _toProcessingQueue.Enqueue(state);
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        /// 
        #endregion
        public bool GetGUIMessage(out IProcessingState state, int timeout = 0)
        {
            int counter = 0;
            while (counter <= timeout)
            {
                if (_toProcessingQueue.Count != 0)
                {
                    state = _toProcessingQueue.Dequeue();
                    return false;
                }
                else
                {
                    counter += 5;
                    Thread.Sleep(5);
                }
            }

            state = null;
            return true;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="message"></param>
        /// 
        #endregion
        public void SendProcessingEvent(IGUIMessage message)
        {
            ToGUIEvent(this, message);
        }        
    }
}
