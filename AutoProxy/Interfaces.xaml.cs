using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
