using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using NativeWifi;

namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ProxyRule> rules = new ObservableCollection<ProxyRule>();
        List<string> ssids = new List<string>();
        Thread poller;
        public MainWindow()
        {
            InitializeComponent();
            
            if (!File.Exists("rules.bin"))
            {
                var ostream = new FileStream("rules.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                var oformatter = new BinaryFormatter();
                oformatter.Serialize(ostream, rules);
                ostream.Close();
            }
            
            var formatter = new BinaryFormatter();
            var stream = new System.IO.FileStream("rules.bin", FileMode.Open, FileAccess.Read);
            rules = (ObservableCollection<ProxyRule>)formatter.Deserialize(stream);
            stream.Close();
            rule_list.ItemsSource = rules;
            //TODO: Periodically check for SSIDs and change proxy accordingly
            poller = new Thread(new Poller(ref rules).StartPolling);
            poller.Start();
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
            rules.Add(newrule);
            WriteRules();                        
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            poller.Abort();
            WriteRules();
        }
    }
}
