using System;
using System.Windows;

namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for AddRule.xaml
    /// </summary>
    public partial class AddRule : Window
    {
        ProxyRule returned_rule;
        public AddRule()
        {
            InitializeComponent();
        }

        public AddRule(ProxyRule SourceRule)
        {
            TextBoxSsid.Text = SourceRule.SSID;
            TextBoxPort.Text = SourceRule.Proxy.Port.ToString();
            TextBoxProxy.Text = SourceRule.Proxy.Ip;

            CheckBoxEnabled.IsChecked = SourceRule.Enabled;
            if (!SourceRule.Enabled) unchecked_enabled(null, null);
            CheckBoxDefault.IsChecked = SourceRule.Default;
            if (SourceRule.Default) checked_def(null, null);

        }

        private void click_Cancel(object sender, RoutedEventArgs e)
        {
            returned_rule = null;
            Close();
        }

        public ProxyRule GetRule()
        {
            return returned_rule;
        }
        private void click_AddRule(object sender, RoutedEventArgs e)
        {
            int port;
            try 
            {
                port = Int32.Parse(TextBoxPort.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Port is not a number! Please enter a valid port number then try again.");
                return;
            }

            bool def = (bool)CheckBoxDefault.IsChecked; //should never be null

            string ssid;
            if (TextBoxSsid.IsEnabled)
                if (TextBoxSsid.Text == "")
                    ssid = "Put something here"; //TODO: Check for valid SSID
                else
                    ssid = TextBoxSsid.Text;
            else
                ssid = "";

            string host;
            if (TextBoxProxy.IsEnabled)
                if (TextBoxProxy.Text == "")
                    host = "localhost"; //TODO: Check for valid host
                else
                    host = TextBoxProxy.Text;
            else
                host = "";

            returned_rule = new ProxyRule(new ProxyHost(host, port), ssid);
            this.Close();
        }

        private void checked_def(object sender, RoutedEventArgs e)
        {
            TextBoxSsid.Text = "";
            TextBoxSsid.IsEnabled = false;
        }

        private void unchecked_def(object sender, RoutedEventArgs e)
        {         
            TextBoxSsid.IsEnabled = true;
        }

        private void checked_enabled(object sender, RoutedEventArgs e)
        {
            TextBoxProxy.IsEnabled = true;
        }

        private void unchecked_enabled(object sender, RoutedEventArgs e)
        {
            TextBoxProxy.Text = "";
            TextBoxPort.Text = "0";
            TextBoxProxy.IsEnabled = false;
        }
    }
}