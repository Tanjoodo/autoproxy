using System;
using System.Text;
using System.Collections.ObjectModel;

using System.Timers;
using NativeWifi;
namespace AutoProxy
{
    public class SsidChangedEventArgs : EventArgs
    {
        public string NewSsid { get; set; }
    }

    public static class Poller
    {
        static WlanClient.WlanInterface _wlanInterface = null;
        static string _ssid = "";

        static public event EventHandler SSIDChanged;

        public static void OnSSIDChanged(SsidChangedEventArgs e)
        {
            EventHandler handler = SSIDChanged;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void SetInterface(WlanClient.WlanInterface wlanInterface)
        {
            if (_wlanInterface == null)
                _wlanInterface = wlanInterface;
            else
            {
                StopPolling(); //detach Poll() from old interface
                _wlanInterface = wlanInterface;
                StartPolling();
            }
        }

        public static void StartPolling()
        {
            if (_wlanInterface != null)
            {
                _wlanInterface.WlanConnectionNotification += Poll;
                Poll();
            }
            else
                throw new NullReferenceException();
        }

        public static void StopPolling()
        {
            if (_wlanInterface != null)
                _wlanInterface.WlanConnectionNotification -= Poll;
            else
                throw new NullReferenceException("Wlan interface was not set");
        }

        static void Poll(Wlan.WlanNotificationData notifyData, Wlan.WlanConnectionNotificationData connNotifyData)
        {
            if (notifyData.notificationSource == Wlan.WlanNotificationSource.ACM)
            {
                if ((Wlan.WlanNotificationCodeAcm)notifyData.notificationCode == Wlan.WlanNotificationCodeAcm.ConnectionComplete ||
                    (Wlan.WlanNotificationCodeAcm)notifyData.notificationCode == Wlan.WlanNotificationCodeAcm.Disconnected)
                    Poll();
            }
            
        }

        public static void Poll()
        {            
            try
            {
                _ssid = new String
                    (
                        ASCIIEncoding.ASCII.GetChars
                        (
                            _wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid.SSID,
                            0,
                            (int)_wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid.SSIDLength
                        )
                    );
            }
            catch
            {
                _ssid = "";
            }

            OnSSIDChanged(new SsidChangedEventArgs{NewSsid = _ssid});
        }

        public static string GetSsid()
        {
            return _ssid;
        }

    }
}