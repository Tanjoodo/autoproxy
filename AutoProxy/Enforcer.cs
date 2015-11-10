using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NativeWifi;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace AutoProxy
{
    static class Enforcer
    {
        [DllImport("wininet.dll")]
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        static bool _settingsReturn, _refreshReturn;
        static RegistryKey RegKey;
        
        public static void Init()
        {
            RegKey = Registry.CurrentUser.
                OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            Poller.SSIDChanged += Poller_SSIDChanged;
            Rules.RulesChanged += Rules_RulesChanged;
            Enforce();
        }

        static void Rules_RulesChanged(object sender, EventArgs e)
        {
            Enforce();
        }

        static void Poller_SSIDChanged(object sender, EventArgs e)
        {
            Enforce();
        }
        public static void Enforce()
        {
            Rules.RulesChanged -= Rules_RulesChanged;
            var rule = Rules.FindRule(Poller.GetSsid());
            if (rule != null)
            {
                ApplyNotNullRule(rule);
            }
            else
            {
                ApplyDefaultRule();
            }
            Rules.RulesChanged += Rules_RulesChanged;
        }

        static void ApplyNotNullRule(ProxyRule rule)
        {
            if ((int)RegKey.GetValue("ProxyEnable") != 1 || (string)RegKey.GetValue("ProxyServer") != rule.Proxy.Host)
            {
                if (!rule.Enabled)
                    RegKey.SetValue("ProxyEnable", 0);
                       
                RegKey.SetValue("ProxyEnable", 1);
                RegKey.SetValue("ProxyServer", rule.Proxy.Host);
                UpdateSettings();

                ShowBalloonTip();
                
            }
        }

        static void ApplyDefaultRule()
        {
            var rule = Rules.FindRule("");
            if (rule != null)
                ApplyNotNullRule(rule);
            else
                ApplyNotNullRule(ProxyRule.Disabled);
                
        }

        static void UpdateSettings()
        {
            _settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            _refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        static void ShowBalloonTip()
        {
            string ssid = Poller.GetSsid();
            string balloonTitle, balloonContent;

            if ((int)RegKey.GetValue("ProxyEnable") == 1)
                balloonContent = "Proxy changed to: " + (string)RegKey.GetValue("ProxyServer") + '.';
            else
                balloonContent = "Proxy disabled.";
            
            if (ssid != "")
                balloonTitle = "Now connected to " + ssid;
            else 
                balloonTitle = "Disconnected from Wifi";

            App._taskbarIcon.ShowBalloonTip(balloonTitle, balloonContent, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }
    }
}
