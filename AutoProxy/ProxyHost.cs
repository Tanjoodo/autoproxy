using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoProxy
{
    [Serializable]
    public class ProxyHost
    {
        private string _host; //contains IP and port
        public string Host { get { return _host; } }
        public ProxyHost (string Host)
        {
            this._host = Host;
        }
        
        public ProxyHost (string IP, int port)
        {
            this._host = IP + ':' + port.ToString();
        }
        
    }
}
