using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using NativeWifi;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace AutoProxy
{
    static class Settings
    {
        [DllImport("wininet.dll")]
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        static bool _settingsReturn, _refreshReturn;
        static RegistryKey RegKey = Registry.CurrentUser.
                                        OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

        static Timer _timer;
        static string _oldProxyServer;
        static int _oldProxyEnable;

        static public event EventHandler SettingsChanged;
        
        static void OnSettingsChanged()
        {
            EventHandler handler = SettingsChanged;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

        public static int ProxyEnable
        {
            get { return (int)RegKey.GetValue("ProxyEnable"); }
            set { RegKey.SetValue("ProxyEnable", value); }
        }

        public static string ProxyServer
        {
            get { return (string)RegKey.GetValue("ProxyServer"); }
            set { RegKey.SetValue("ProxyServer", value); }
        }

        public static void Init()
        {
            _timer = new Timer(100);
            _timer.Elapsed += (object sender, ElapsedEventArgs e) => CheckChanges();
            _oldProxyEnable = ProxyEnable;
            _oldProxyServer = ProxyServer;
            _timer.Start();
        }

        static void CheckChanges()
        {
            if (ProxyEnable != _oldProxyEnable || ProxyServer != _oldProxyServer)
            {
                _oldProxyServer = ProxyServer;
                _oldProxyEnable = ProxyEnable;
                OnSettingsChanged();
            }
        }

        public static void UpdateSettings()
        {
            _settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            _refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

    }
}
