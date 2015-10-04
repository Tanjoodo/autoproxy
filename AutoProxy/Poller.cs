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
        //TODO: Pair each poller with an iterface
        
        private ObservableCollection<ProxyRule> _rules;
        private string _ssid = "";        
        private string _interface = null;

        private Object l = new Object();
        public Poller(ref ObservableCollection<ProxyRule>  Rules, string Interface)
        {
            _rules = Rules;
            _rules.CollectionChanged += RulesChanged;
            _interface = Interface;
        }
        public void RulesChanged(Object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _rules = (ObservableCollection<ProxyRule>)sender;
        }
        public void StartPolling()
        {
            var poll_timer = new System.Timers.Timer(100);
            poll_timer.Elapsed += TimerEvent;
            poll_timer.Start();         
        }
        public void TimerEvent(Object source, ElapsedEventArgs e)
        {
            if (_interface == null)
                return;
            var thread = new Thread(this.Poll); 
            thread.Start();
            thread.Join();
        }
        private void Poll()
        {
            if (Monitor.TryEnter(l)) //Allow only one thread to enter this section at a time, should be better than lock() because as far as I understand it,
                                     //lock will queue all the threads, and that would cause problems.
            {
                UpdateSSID();


                var rule = FindRule();
                if (rule != null)
                {
                    if ((string)RegKey.GetValue("ProxyServer") != rule.Proxy.Host || (int)RegKey.GetValue("ProxyEnable") != 1)
                        ChangeProxy(rule.Proxy);
                }
                else
                {
                    if ((int)RegKey.GetValue("ProxyEnable") != 0)
                        DisableProxy();
                }

                Monitor.Exit(l);
            }
            else
                return;
        }

        private ProxyRule FindRule()
        {
            foreach (var y in _rules)
                if (y.SSID == _ssid)
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

        void UpdateSSID()
        {
            WlanClient wlan = null;
            try
            {
                wlan = new WlanClient();
            }
            catch (Exception)
            {
                return;
            }
            foreach (var inter in wlan.Interfaces)
            {
                if (inter.InterfaceDescription == _interface)
                {
                    try
                    {
                        _ssid = new String(Encoding.ASCII.GetChars(inter.CurrentConnection.wlanAssociationAttributes.dot11Ssid.SSID, 0,
                            (int)inter.CurrentConnection.wlanAssociationAttributes.dot11Ssid.SSIDLength));
                    }
                    catch (Exception)
                    {
                        _ssid = null; //not connected to a network
                    }
                    return;
                }
            }
        }
    }
}