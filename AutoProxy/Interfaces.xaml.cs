using System.Collections.Generic;
using System.Windows;

using NativeWifi;
namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for Interfaces.xaml
    /// </summary>
    public partial class Interfaces : Window
    {
        List<string> interfaces = new List<string>();
        private string result;
        public Interfaces()
        {
            InitializeComponent();

            var wlan = new WlanClient();
            foreach (var inter in wlan.Interfaces)
            {
                interfaces.Add(inter.InterfaceDescription);
            }

            ComboBox_interfaces.ItemsSource = interfaces;
            if (interfaces.Count > 0)
                ComboBox_interfaces.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            result = (string)ComboBox_interfaces.SelectedValue;
            Close();
        }

        public string GetInterface()
        {
            return result;
        }
    }
}
