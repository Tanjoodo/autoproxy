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
        public static void Init()
        {
            Poller.SSIDChanged += (object sender, EventArgs e) => Enforce();
            Rules.RulesChanged += (object Sender, EventArgs e) => Enforce();
            Settings.SettingsChanged += (object Sender, EventArgs e) => Enforce();
            Enforce();
        }

        public static void Enforce()
        {
            var rule = Rules.FindRule(Poller.GetSsid());
            if (rule != null)
            {
                ApplyNotNullRule(rule);
            }
            else
            {
                ApplyDefaultRule();
            }
        }

        static void ApplyNotNullRule(ProxyRule rule)
        {
            if (Settings.ProxyEnable != 1 || Settings.ProxyServer != rule.Proxy.Host)
            {
                if (!rule.Enabled)
                {
                    Settings.ProxyEnable = 0;
                }
                else
                {
                    Settings.ProxyEnable = 1;
                    Settings.ProxyServer = rule.Proxy.Host;
                }

                Settings.UpdateSettings();
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

        static void ShowBalloonTip()
        {
            string ssid = Poller.GetSsid();
            string balloonTitle, balloonContent;

            if (Settings.ProxyEnable == 1)
                balloonContent = "Proxy changed to: " + Settings.ProxyServer + '.';
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
