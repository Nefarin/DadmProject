using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.IO;
using EKG_Project.Architecture.GUIMessages;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
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
        /// 
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
                Console.WriteLine(process.Modules.AnalysisName);
                FileProcessor fp = new FileProcessor(converter, process.Modules.AnalysisName, 400000);
                process.FileProcessor = fp;
                timeoutState = new ProcessFile();

                //converter.ConvertFile(Path);
                //converter.SaveResult();
                //process.Modules.FileLoaded = true;

                //process.Communication.SendProcessingEvent(new FileLoaded());

            }
            catch (Exception e)
            {
                process.Communication.SendProcessingEvent(new FileLoadingError());
                timeoutState = new Idle(5);
            }    
            

        }
    }
}
