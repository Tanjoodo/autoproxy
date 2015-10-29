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
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;
        static RegistryKey RegKey = Registry.CurrentUser.
                OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

        public void Init()
        {

        }
    }
}
