using System.Windows.Controls;
using System.Threading;
using EKG_Project.Architecture;
using EKG_Project.Architecture.GUIMessages;
using System.Collections.Generic;
using EKG_Project.Modules;

namespace EKG_Project.GUI
{
    //For Architects - GUI - do not touch.
    public partial class AnalysisControl : UserControl
    {
        private Window_ECG _parent;
        private TabItem _parentTab;
        private ProcessSync _communication;
        private SynchronizationContext _context;
        private ToGUIDelegate _analysisEvent;
        private string _analysisName;
        private Dictionary<AvailableOptions, ModuleParams> moduleParams;
        private Dictionary<AvailableOptions, bool> isComputed;

        #region Properties

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
        public Window_ECG Parent
        {
            get
            {
                return _parent;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public TabItem ParentTab
        {
            get
            {
                return _parentTab;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public ToGUIDelegate AnalysisEvent
        {
            get
            {
                return _analysisEvent;
            }

            set
            {
                _analysisEvent = value;
            }
        }

        public string AnalysisName
        {
            get
            {
                return _analysisName;
            }

            set
            {
                _analysisName = value;
            }
        }

        #endregion

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        /// <param name="communication"></param>
        /// <param name="parent"></param>
        /// <param name="parentTab"></param>
        /// <param name="context"></param>
        /// 
        #endregion
        public AnalysisControl(ProcessSync communication, Window_ECG parent, TabItem parentTab, SynchronizationContext context)
        {
            _context = context;
            _communication = communication;
            _analysisEvent = new ToGUIDelegate(analysisEventHandler);
            _communication.ToGUIEvent += _analysisEvent;
            _parent = parent;
            _parentTab = parentTab;
            InitializeComponent();
            pdfButton.IsEnabled = false;
            startAnalyseButton.IsEnabled = false;
            moduleParams = new Dictionary<AvailableOptions, ModuleParams>();
            isComputed = new Dictionary<AvailableOptions, bool>();
        }

        public AnalysisControl(ProcessSync communication, Window_ECG parent, TabItem parentTab, SynchronizationContext context, string analysisName)
            : this(communication, parent, parentTab, context)
        {
            this.AnalysisName = this.modulePanel.AnalysisName = analysisName;
        }

        private void analysisEventHandler(object sender, IGUIMessage message)
        {
            _context.Post(new SendOrPostCallback((o) => {
                analyzeEvent(message);
            }), null);
        }
    }
}
