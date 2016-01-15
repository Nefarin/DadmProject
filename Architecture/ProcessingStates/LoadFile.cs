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
                Console.WriteLine("where");
                fileLoader.AnalysisName = process.Modules.AnalysisName;
                Console.WriteLine("where1");
                fileLoader.Validate(Path);
                Console.WriteLine("where2");
                fileLoader.Load(Path);
                Console.WriteLine("where4");

                IECGConverter converter = fileLoader.Converter;
                Console.WriteLine("where5");
                converter.ConvertFile(Path);
                Console.WriteLine("where6");
                converter.SaveResult();
                Console.WriteLine("where7");
                process.Modules.FileLoaded = true;

                process.Communication.SendProcessingEvent(new FileLoaded());

            }
            catch (Exception e)
            {
                process.Communication.SendProcessingEvent(new FileLoadingError());
            }    
            
            timeoutState = new Idle(5);
        }
    }
}
