using System.Windows.Controls;
using System.Threading;
using EKG_Project.Architecture;

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
        }

        private void analysisEventHandler(object sender, ToGUIItem item)
        {
            _context.Post(new SendOrPostCallback((o) => {
                analyzeEvent(item);
            }), null);
        }
    }
}
