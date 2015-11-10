using System;

namespace AutoProxy
{
    [Serializable]
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

        public static ProxyRule Disabled
        {
            get { return new ProxyRule("", 0, ""); }
        }

        public ProxyRule(ProxyHost Proxy, string SSID)
        {
            this.Proxy = Proxy;
            this.SSID = SSID;
            this.Enabled = !Proxy.Host.StartsWith(":"); //Disabled if no proxy specified
            this.Default = SSID == ""; //Default if no SSID specified
        }

        public ProxyRule(string ProxyIP, int ProxyPort, string SSID)
        {
            try
            {
                this.Proxy = new ProxyHost(ProxyIP, ProxyPort);
            }
            catch
            {
                throw;
            }

            this.SSID = SSID;
            this.Enabled = Proxy.Host != "";
            this.Default = SSID == "";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            
            ProxyRule rule2 = obj as ProxyRule;
            if (rule2 == null) return false;

            return this._proxy == rule2._proxy
                && this._ssid == rule2._ssid
                && this._default == rule2._default
                && this._enabled == rule2._enabled;
        }
    }
}
