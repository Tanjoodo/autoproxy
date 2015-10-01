using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows;
using System.Timers;
using System.Threading;
using NativeWifi;
namespace AutoProxy
{
    class Poller
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        bool settingsReturn, refreshReturn;
        RegistryKey RegKey = Registry.CurrentUser.
                OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
        
        private ObservableCollection<ProxyRule> _rules;
        private List<string> _ssids = new List<string>();
        private string _last_ssid = "";

        private Object l = new Object();
        public Poller(ref ObservableCollection<ProxyRule>  Rules)
        {
            _rules = Rules;
            _rules.CollectionChanged += RulesChanged;
            
        }
        public void RulesChanged(Object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _rules = (ObservableCollection<ProxyRule>)sender;
        }
        public void StartPolling()
        {
            var poll_timer = new System.Timers.Timer(1000);
            poll_timer.Elapsed += TimerEvent;
            poll_timer.Start();     
        }
        public void TimerEvent(Object source, ElapsedEventArgs e)
        {
            var thread = new Thread(this.Poll);
            thread.Start();
            thread.Join();
        }
        private void Poll()
        {
            //Update SSIDs
            //Compare SSID to rules
            //If found apply rule
            lock (l)
            {
                UpdateSSIDs();

                ProxyRule rule = null;
                bool ssid_changed = true;
                foreach (var x in _ssids)
                    if (_last_ssid == x)
                    {
                        ssid_changed = false;
                        break;
                    }

                if (ssid_changed)
                {
                    rule = FindRule();
                    if (null != rule)
                        ChangeProxy(rule.Proxy);
                    else
                        DisableProxy();

                    _last_ssid = _ssids[0]; //TODO: don't assume you're only connected to one SSID. 
                }
            }
        }

        private ProxyRule FindRule()
        {
            foreach (var x in _ssids)
                foreach (var y in _rules)
                    if (y.SSID == x)
                        return y;
            return null;
        }

        void UpdateSettings()
        {
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        void ChangeProxy(ProxyHost host)
        {
            RegKey.SetValue("ProxyEnable", 1);
            RegKey.SetValue("ProxyServer", host.Host);
            UpdateSettings();
            MessageBox.Show("Changed proxy to " + host.Host);

        }

        void DisableProxy()
        {
            RegKey.SetValue("ProxyEnable", 0);
            UpdateSettings();
            MessageBox.Show("Disabled Proxy");
        }

        void UpdateSSIDs()
        {
            if (_ssids.Count > 0) _ssids.Clear();
            WlanClient wlan = null;
            try
            {
                wlan = new WlanClient();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while initializing WlanClient (I probably can't see your Wifi interface):\n" + e.ToString()); //TODO: more graceful exception handling.
                return;
            }

            foreach (var inter in wlan.Interfaces)
            {
                Wlan.Dot11Ssid ssid;
                try
                {
                    ssid = inter.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return;
                }

                _ssids.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
            }
        }
    }
}