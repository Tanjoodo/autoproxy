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
        public MainWindow()
        {
            InitializeComponent();
            
            //TODO:
            /* Look for rules file
             * If not found create an empty one
             * At the end of the program write rules to disk
             */
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
            UpdateSSIDs();

        }

        void WriteRules()
        {
            var ostream = new FileStream("rules.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            var formatter = new BinaryFormatter();
            formatter.Serialize(ostream, rules);
            ostream.Close();
        }
        void UpdateSSIDs()
        {
            ssids.Clear();
            WlanClient wlan = null;
            try
            {
                wlan = new WlanClient();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error while initializing WlanClient (I probably can't see your Wifi interface):\n" + e.ToString()); //TODO: more graceful exception handling.
                return;
            }
            
            foreach(var inter in wlan.Interfaces)
            {
                Wlan.Dot11Ssid ssid;
                try 
                {
                    ssid = inter.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return;
                }

                ssids.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
            }
        }

        private void click_AddRule(object sender, RoutedEventArgs e)
        {
            AddRule window = new AddRule();
            ProxyRule newrule = null;
            window.ShowDialog();
            newrule = window.GetRule();
            rules.Add(newrule);
            WriteRules();
            
            //TODO: Add rule to DB
        }
    }
}
