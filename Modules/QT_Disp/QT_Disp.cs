using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.QT_Disp
{
    public partial class QT_Disp : IModule
    {
        private bool _ended;
        private bool _aborted;

        //private Basi_Data_Worker _inputWorker;
        private Basic_Data _inputData;

        private QT_Disp_Params _params;
        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            Params = parameters as QT_Disp_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {

            }
            
        }

        public void ProcessData()
        {
            throw new NotImplementedException();
        }

        public double Progress()
        {
            throw new NotImplementedException();
        }

        public bool Runnable()
        {
            throw new NotImplementedException();
        }

        public bool Aborted
        {
            get
            {
                return _aborted;
            }
            set
            {
                _aborted = value;
            }
        }
        public QT_Disp_Params Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }
        
    }
}
