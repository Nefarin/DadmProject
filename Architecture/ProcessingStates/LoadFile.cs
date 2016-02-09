using EKG_Project.IO;
using EKG_Project.Architecture.GUIMessages;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which loads given file.
    /// </summary>
    /// 
    #endregion
    public class LoadFile : IProcessingState
    {
        private string _path;

        public LoadFile(string path)
        {
            Path = path;
        }

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
            }
        }

        #region Documentation
        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        /// 
        #endregion
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            FileLoader fileLoader = new FileLoader();
            try
            {
                fileLoader.AnalysisName = process.Modules.AnalysisName;
                fileLoader.Validate(Path);
                fileLoader.Load(Path);

                IECGConverter converter = fileLoader.Converter;
                converter.ConvertFile(Path);
                process.Converter = converter;
                process.FileProcessor = new FileProcessor(converter, process.Modules.AnalysisName, 5000);
                timeoutState = new ProcessFile();
            }
            catch (Exception e)
            {
                process.Communication.SendProcessingEvent(new FileLoadingError());
                timeoutState = new Idle(5);
            }    
            

        }
    }
}
