using System;
using System.Collections.Generic;
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

using NativeWifi;

namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ProxyRule> rules = new List<ProxyRule>();
        List<string> ssids = new List<string>();
        public MainWindow()
        {
            InitializeComponent();

            rule_list.ItemsSource = rules;
            UpdateSSIDs();
            
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
                MessageBox.Show("Error while initializing WlanClient:" + e.ToString()); //TODO: more graceful exception handling.
                Environment.Exit(1);
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
            
            //TODO: Add rule to DB
        }
    }
}
