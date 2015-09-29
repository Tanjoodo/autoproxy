using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoProxy
{
    public class ProxyRule
    {
        private bool _enabled;
        private bool _default;
        private string _ssid;
        private ProxyHost _proxy;
        public ProxyHost Proxy 
        { 
            get { return _proxy; }
            set { _proxy = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public string SSID
        {
            get { return _ssid; }
            set { _ssid = value; }
        }

        public ProxyRule (ProxyHost Proxy, string SSID)
        {
            this.Proxy = Proxy;
            this.SSID = SSID;
            this.Enabled = !Proxy.Host.StartsWith(":"); //Disabled if no proxy specified
            this.Default = SSID != ""; //Default if no SSID specified
        }

        public ProxyRule (string ProxyIP, int ProxyPort, string SSID)
        {
            this.Proxy = new ProxyHost(ProxyIP, ProxyPort);
            this.SSID = SSID;
            this.Enabled = !Proxy.Host.StartsWith(":");
            this.Default = SSID != "";
        }
    }
}
