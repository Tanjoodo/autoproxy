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
        ObservableCollection<ProxyRule> rules = new ObservableCollection<ProxyRule>();
        Thread poller;
        public static TaskbarIcon tb_icon = new TaskbarIcon();
        public MainWindow()
        {
            InitializeComponent();
            
            if (!File.Exists("rules.bin"))
                WriteRules();
            
            var formatter = new BinaryFormatter();
            var stream = new System.IO.FileStream("rules.bin", FileMode.Open, FileAccess.Read);
            rules = (ObservableCollection<ProxyRule>)formatter.Deserialize(stream);
            stream.Close();
        
            tb_icon.Icon = new System.Drawing.Icon("favicon.ico");
            tb_icon.TrayMouseDoubleClick += Tray_DoubleClick;
            this.StateChanged += Window_StateChanged;
            rule_list.ItemsSource = rules;
            //TODO: Show what SSID it thinks it's connected to on the window
            string inter;
            var client = new WlanClient();
            if(client.Interfaces.Count() > 1)
            {
                var inters = new Interfaces();
                inters.ShowDialog();
                inter = inters.GetInterface();
            }
            else if (client.Interfaces.Count() == 1)
            {
                inter = client.Interfaces[0].InterfaceDescription;
            }
            else
            {
                inter = null;
            }

            if (inter != null)
            {
                StartPoller(inter);
            }
        }

        void WriteRules()
        {
            var ostream = new FileStream("rules.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            var formatter = new BinaryFormatter();
            formatter.Serialize(ostream, rules);
            ostream.Close();
        }
        
        private void click_AddRule(object sender, RoutedEventArgs e)
        {
            AddRule window = new AddRule();
            ProxyRule newrule = null;
            window.ShowDialog();
            newrule = window.GetRule();
            if (newrule != null) rules.Add(newrule);
            WriteRules();                        
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            poller.Abort();
            WriteRules();
        }

        private void StartPoller(string inter)
        {
            poller = new Thread(new Poller(ref rules, inter).StartPolling);
            poller.Start();
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
