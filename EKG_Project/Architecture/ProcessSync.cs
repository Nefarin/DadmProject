using System.Collections.Generic;
using System.Threading;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// TODO
    /// </summary>
    /// 
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

        private Queue<ToProcessingItem> _toProcessingQueue;

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public ProcessSync()
        {
            _toProcessingQueue = new Queue<ToProcessingItem>();
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="item"></param>
        /// 
        #endregion
        public void sendGUIMessage(ToProcessingItem item)
        {
            _toProcessingQueue.Enqueue(item);
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// 
        #endregion
        public ToProcessingItem getGUIMessage(int timeout = 0)
        {
            int counter = 0;
            while (counter <= timeout)
            {
                if(_toProcessingQueue.Count != 0)
                {
                    return _toProcessingQueue.Dequeue();
                }
                else
                {
                    counter += 5;
                    Thread.Sleep(5);
                }
            }

            return new ToProcessingItem(AnalysisState.TIMEOUT, null);
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="item"></param>
        /// 
        #endregion
        public void SendProcessingEvent(ToGUIItem item)
        {
            ToGUIEvent(this, item);
        }        
    }
}
