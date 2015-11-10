using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace EKG_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App parentApp;
        private List<TabItem> _tabItems; 

        public MainWindow(App app)
        {
            parentApp = app;
            _tabItems = new List<TabItem>();

            InitializeComponent();
           
            TabItem tabAdd = new TabItem();
            tabAdd.Header = "+";
            _tabItems.Add(tabAdd);

            analysisTabControl.DataContext = _tabItems;
            analysisTabControl.SelectedIndex = 0;
        }

        override protected void OnClosing(CancelEventArgs closeArgs)
        {
            closeArgs.Cancel = true;
            this.exitClick(this, null);
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            parentApp.Shutdown();
        }

        private void newAnalysis(object sender, RoutedEventArgs e)
        {
            analysisTabControl.DataContext = null;
            addTabItem();
            analysisTabControl.DataContext = _tabItems;
            analysisTabControl.SelectedIndex = _tabItems.Count - 2;
        }

        private TabItem addTabItem()
        {
            int count = _tabItems.Count;

            TabItem tab = new TabItem();
            tab.Header = string.Format("Analysis {0}", count);
            tab.Name = string.Format("analysis{0}", count);
            tab.HeaderTemplate = analysisTabControl.FindResource("tabHeader") as DataTemplate;
            UserControl analysisControl = new AnalysisControl();
            tab.Content = analysisControl;

            _tabItems.Insert(count - 1, tab);
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
                    analysisTabControl.DataContext = _tabItems;
                    analysisTabControl.SelectedIndex = _tabItems.Count - 2;
                }
            }
        }
    }
}
