using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using EKG_Project.Architecture;
using EKG_Project.Architecture.GUIMessages;
using EKG_Project.Architecture.ProcessingStates;

namespace EKG_Project.GUI
{
    //For Architects - GUI - do not touch.

    public partial class Window_ECG : Window
    {
        private App _parentApp;
        private TabContainer _tabContainer;
        private SynchronizationContext _context;

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// 
        #endregion
        public Window_ECG(App app)
        {
            initMembers(app);
            InitializeComponent();
            initWindow();
            initTabs();
        }

        #region Initialization Functions
        private void initMembers(App app)
        {
            _context = SynchronizationContext.Current;
            _tabContainer = new TabContainer();
            _parentApp = app;
        }
        private void initWindow()
        {
            this.Title = "ECG Analysis";
        }
        private void initTabs()
        {
            TabItem tabAdd = new TabItem();
            tabAdd.Header = "+";
            _tabContainer.TabItems.Add(tabAdd);
            analysisTabControl.DataContext = _tabContainer.TabItems;
        }

        #endregion

        #region Exit Functions
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="closeArgs"></param>
        /// 
        #endregion
        override protected void OnClosing(CancelEventArgs closeArgs)
        {
            closeArgs.Cancel = true;
            this.exitClick(this, null);
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            closeTabs();
            _parentApp.Shutdown();
        }
        #endregion

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// 
        #endregion
        public void closeAnalysisTab(TabItem item)
        {
            int tabIndex = _tabContainer.TabItems.IndexOf(item);
            _tabContainer.ThreadList[tabIndex].Abort();
            _tabContainer.ThreadList.RemoveAt(tabIndex);
            analysisTabControl.DataContext = null;
            _tabContainer.TabItems.RemoveAt(tabIndex);
            analysisTabControl.DataContext = _tabContainer.TabItems;

            analysisTabControl.SelectedIndex = tabIndex - 1;

        }
        private void closeTabs()
        {
            for (int i = 0; i < _tabContainer.ThreadList.Count; i++)
            {
                _tabContainer.CommunicationList[i].SendGUIMessage(new Abort());
            }
        }

        private void newAnalysis(object sender, RoutedEventArgs e)
        {
            analysisTabControl.DataContext = null;
            addTabItem();
            analysisTabControl.DataContext = _tabContainer.TabItems;
            analysisTabControl.SelectedIndex = _tabContainer.TabItems.Count - 2;
        }

        private void closeAnalysis(object sender, RoutedEventArgs e)
        {
            int currentTabIndex = analysisTabControl.SelectedIndex;
            if (currentTabIndex != -1)
            {
                _tabContainer.CommunicationList[currentTabIndex].SendGUIMessage(new Abort());
            }

        }

        private TabItem addTabItem()
        {
            int count = _tabContainer.TabItems.Count;

            TabItem tab = new TabItem();
            tab.Header = string.Format("Analysis {0}", count);
            tab.Name = string.Format("analysis{0}", count);
            tab.HeaderTemplate = analysisTabControl.FindResource("tabHeader") as DataTemplate;

            ProcessSync communication = new ProcessSync();
            _tabContainer.CommunicationList.Insert(count - 1, communication);

            UserControl analysisControl = new AnalysisControl(communication, this, tab, _context);
            _tabContainer.AnalysisControlList.Insert(count - 1, analysisControl);
            tab.Content = analysisControl;

            Processing ecgAnalysis = new Processing(communication);
            Thread analysisThread = new Thread(ecgAnalysis.run);
            _tabContainer.ThreadList.Insert(count - 1, analysisThread);
            analysisThread.Start();


            _tabContainer.TabItems.Insert(count - 1, tab);
            return tab;
        }

        private void analysisTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = analysisTabControl.SelectedItem as TabItem;

            if (tab != null && tab.Header != null)
            {
                if (tab.Header.Equals("+"))
                {
                    analysisTabControl.DataContext = null;
                    addTabItem();
                    analysisTabControl.DataContext = _tabContainer.TabItems;
                    analysisTabControl.SelectedIndex = _tabContainer.TabItems.Count - 2;
                }
            }
        }

    }
}
