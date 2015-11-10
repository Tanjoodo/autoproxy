using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using NativeWifi;
using Hardcodet.Wpf.TaskbarNotification;
namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Rules.BindListView(ref rule_list);
        }
        
        private void click_AddRule(object sender, RoutedEventArgs e)
        {
            var dialog = new AddRule();
            dialog.ShowDialog();
            var rule = dialog.GetRule();
            if (rule != null)
            {
                try
                {
                    Rules.AddRule(rule);
                }
                catch (Exception except)
                {
                    MessageBox.Show(except.Message);
                }

                try
                {
                    Rules.WriteRules("rules.bin");
                }
                catch 
                {
                    MessageBox.Show("Error while writing rules");
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }

            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private void Tray_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }
    }
}
