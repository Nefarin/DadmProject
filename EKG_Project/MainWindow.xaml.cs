using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EKG_Project.GUI;
using EKG_Project.Architecture;

namespace EKG_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App parentApp;
        private TabContainer _tabContainer;

        public MainWindow(App app)
        {
            _tabContainer = new TabContainer();
            parentApp = app;

            InitializeComponent();
           
            TabItem tabAdd = new TabItem();
            tabAdd.Header = "+";
            _tabContainer.TabItems.Add(tabAdd);

            analysisTabControl.DataContext = _tabContainer.TabItems;
        }

        override protected void OnClosing(CancelEventArgs closeArgs)
        {
            closeArgs.Cancel = true;
            this.exitClick(this, null);
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            closeTabs();
            parentApp.Shutdown();
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
                _tabContainer.CommunicationList[currentTabIndex].sendGUIMessage(new GUI_To_Analysis_Item(GUI_To_Analysis_Command.STOP_ANALYSIS, null));
            }

        }

        private TabItem addTabItem()
        {
            int count = _tabContainer.TabItems.Count;

            TabItem tab = new TabItem();
            tab.Header = string.Format("Analysis {0}", count);
            tab.Name = string.Format("analysis{0}", count);
            tab.HeaderTemplate = analysisTabControl.FindResource("tabHeader") as DataTemplate;
            
            ECG_Communication communication = new ECG_Communication();
            _tabContainer.CommunicationList.Insert(count - 1, communication);


            UserControl analysisControl = new AnalysisControl(communication, this, tab);
            _tabContainer.AnalysisControlList.Insert(count - 1, analysisControl);
            tab.Content = analysisControl;

            ECG_Analysis ecgAnalysis = new ECG_Analysis(communication);
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
            foreach (var thread in _tabContainer.ThreadList)
            {
                thread.Abort();
            }
        }


    }

    #region TabContainer
    public class TabContainer
    {
        private List<TabItem> _tabItems;
        private List<ECG_Communication> _communicationList;
        private List<ECG_Analysis> _ecgAnalysisList;
        private List<UserControl> _analysisControlList;
        private List<Thread> _threadList;

        public TabContainer()
        {
            _tabItems = new List<TabItem>();
            _communicationList = new List<ECG_Communication>();
            _ecgAnalysisList = new List<ECG_Analysis>();
            _analysisControlList = new List<UserControl>();
            _threadList = new List<Thread>();
        }

        #region Properties
        public List<TabItem> TabItems
        {
            get
            {
                return _tabItems;
            }

            set
            {
                _tabItems = value;
            }
        }

        public List<ECG_Communication> CommunicationList
        {
            get
            {
                return _communicationList;
            }

            set
            {
                _communicationList = value;
            }
        }

        public List<ECG_Analysis> EcgAnalysisList
        {
            get
            {
                return _ecgAnalysisList;
            }

            set
            {
                _ecgAnalysisList = value;
            }
        }

        public List<UserControl> AnalysisControlList
        {
            get
            {
                return _analysisControlList;
            }

            set
            {
                _analysisControlList = value;
            }
        }

        public List<Thread> ThreadList
        {
            get
            {
                return _threadList;
            }

            set
            {
                _threadList = value;
            }
        }
        #endregion
    }
    #endregion
}
