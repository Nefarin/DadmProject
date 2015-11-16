using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules
{
    #region Documentation
    /// <summary>
    /// Common interface for modules.
    /// </summary>
    /// 
    #endregion
    public interface IModule
    {
        #region Documentation
        /// <summary>
        /// Initializes module.
        /// </summary>
        /// 
        #endregion
        void Init(ModuleParams parameters);

        #region Documentation
        /// <summary>
        /// As name says.
        /// </summary>
        /// <param name="numberOfSamples"></param>
        /// 
        #endregion
        void ProcessData(int numberOfSamples);

        #region Documentation
        /// <summary>
        /// Returns current progress in percents.
        /// </summary>
        /// <returns></returns>
        /// 
        #endregion
        double Progress();

        #region Documentation
        /// <summary>
        /// Check if module processed all data points.
        /// </summary>
        /// <returns></returns>
        /// 
        #endregion
        bool Ended();

        #region Documentation
        /// <summary>
        /// Aborts current module.
        /// </summary>
        /// 
        #endregion
        void Abort();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Runnable();
    }
}
