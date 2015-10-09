using System;

namespace AutoProxy
{

    public class IpParseException : ArgumentException
    {
        public IpParseException(string message, string paramName) : base(message, paramName) { }
    }
    
    public class PortParseException : ArgumentException
    {
        public PortParseException(string message, string paramName) : base(message, paramName) { }
    }
    
    public class PortRangeException : ArgumentException {
        public PortRangeException(string message, string paramName) : base(message, paramName) { }
    }

    [Serializable]
    public class ProxyHost
    {
        private string _ip;
        private int _port;

        public string Host
        {
            get { return _ip + _port.ToString(); }
        }
        public string Ip
        {
            get { return _ip; }
        }
        public int Port
        {
            get { return _port; }
        }

        /// <summary>
        /// Creates a new ProxyHost object from a URL-like string.
        /// </summary>
        /// <param name="Host">Host string in the format "IP/Domain:Port". Example: "127.0.0.1:80"</param>
        /// <exception cref="AutoProxy.IpParseException"></exception>
        /// <exception cref="AutoProxy.PortParseException"></exception>
        /// <exception cref="AutoProxy.PortRangeException"></exception>
        public ProxyHost (string Host)
        {
            try
            {
                _ip = Host.Split(new char[] { ':' })[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new IpParseException("Could not get URL/IP", "Host");
            }

            try
            {
                _port = UInt16.Parse(Host.Split(new Char[] { ':' })[1]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new PortParseException("Could not get port", "Host");
            }
            catch (FormatException)
            {
                throw new PortParseException("Port is not in correct format", "Host");
            }
            catch (OverflowException)
            {
                throw new PortRangeException("Not a valid port number", "Host");
            }

        }
        /// <summary>
        /// Creates a new ProxyHost object from an IP/domain string and an integer port
        /// </summary>
        /// <param name="IP">IP or domain name of proxy server</param>
        /// <param name="Port">Port number of proxy server</param>
        /// <exception cref="AutoProxy.PortRangeException">Thrown if port is not in valid port range (0 to 65535)</exception>
        public ProxyHost (string IP, int Port)
        {
            _ip = IP;
            if (Port >= UInt16.MinValue && Port <= UInt16.MaxValue) _port = Port;
            else throw new PortRangeException("Port is not a valid port number", "Port");
        }
        
    }
}
