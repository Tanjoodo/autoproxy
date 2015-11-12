using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;
using NativeWifi;
namespace AutoProxy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary
    public partial class App : Application
    {
        public static TaskbarIcon _taskbarIcon = new TaskbarIcon();
        static Window _mainWin = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TaskbarIconInit();

            PollerInit();
            RulesInit();
            Settings.Init();
            Enforcer.Init();
            if (!e.Args.Contains("-q"))
            {
                _mainWin = new MainWindow();
                _mainWin.Show();
            }

        } 

        private static void onTrayMouseDoubleClick(object sender, EventArgs e)
        {
            _mainWin = new MainWindow();
            _mainWin.Show();      
        }
        
        private static void PollerInit()
        {
            WlanClient client;
            WlanClient.WlanInterface inter = null;
            try
            {
                client = new WlanClient();

            }
            catch
            {
                client = null;
            }

            if (client != null)
            {
                if (client.Interfaces.Count() > 1)
                {
                    var inters = new Interfaces();
                    inters.ShowDialog();
                    string interName = inters.GetInterface();
                    foreach (var x in client.Interfaces)
                        if (x.InterfaceName == interName)
                        {
                            inter = x;
                            break;
                        }
                }
                else if (client.Interfaces.Count() == 1)
                {
                    Poller.SetInterface(client.Interfaces[0]);
                }
            }

            try
            {
                Poller.StartPolling();
            }
            catch
            {
                // do something here
            }
        }

        private static void RulesInit()
        {
            Rules.Init();
            try
            {
                Rules.LoadRules("rules.bin");
            }
            catch (System.IO.FileNotFoundException)
            {
                Rules.WriteRules("rules.bin");
                Rules.LoadRules("rules.bin");
            }
        }
            
        private static void TaskbarIconInit()
        {
            _taskbarIcon.Icon = new System.Drawing.Icon("favicon.ico");
            _taskbarIcon.TrayMouseDoubleClick += onTrayMouseDoubleClick;
            _taskbarIcon.ContextMenu = new System.Windows.Controls.ContextMenu();
            var exitMenuItem = new System.Windows.Controls.MenuItem();
            exitMenuItem.Header = "Exit applicaton";
            _taskbarIcon.ContextMenu.Items.Add(exitMenuItem);
            exitMenuItem.Click += (object sender, RoutedEventArgs e) => { _taskbarIcon.Dispose(); Application.Current.Shutdown(); };
        }
    }
}
