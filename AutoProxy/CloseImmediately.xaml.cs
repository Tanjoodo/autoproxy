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

namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for CloseImmediately.xaml
    /// </summary>
    public partial class CloseImmediately : Window
    {
        public CloseImmediately() // The name is now a misnomer. It does not close immediately, rather it now stays invisible.
        {
            this.Width = 0;
            this.Height = 0;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ShowInTaskbar = false;
            this.ShowActivated = false;
        }
    }
}
