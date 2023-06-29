using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TunnelFireControl.Dao;
using TunnelFireControl.UserControls;

namespace TunnelFireControl
{
    public delegate void RefreshClockDele();
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string UserName;
        private System.Timers.Timer timer;
        private RefreshClockDele RefreshClockHandle;
        private LoginWindow logWindow;

        public MainWindow()
        {
            InitializeComponent();
            RefreshClockHandle = RefreshClock;
            LiquidLevelDao liquidLevelDao = DaoFactory.liquidLevelDao;
            List<LiquidLevelEntity> liquidLevelEntities = liquidLevelDao.FindAll();
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 1000;
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(RefreshClockHandle);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lblUserName.Content = "当前用户：" + UserName;
            FireControlMonitor fireControlMain = new FireControlMonitor();
            this.gridMain.Children.Add(fireControlMain);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Dispose();
            Application.Current.Shutdown();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (logWindow != null)
            {
                logWindow.Show();
                Dispose();
                this.Close();
            }
        }


        private void Dispose()
        {
            System.Diagnostics.Process[] pro = System.Diagnostics.Process.GetProcessesByName("TunnelFireControl");
            foreach (System.Diagnostics.Process p in pro)
            {
                if (!string.IsNullOrEmpty(p.ProcessName))
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            GC.Collect();
        }

        public void SetLogWindow(LoginWindow logWindow)
        {
            this.logWindow = logWindow;
        }

        /// <summary>
        /// 刷新服务状态和时间表
        /// </summary>
        private void RefreshClock()
        {
            this.lblDatetime.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            elpEcho.Fill = new SolidColorBrush(Colors.Green);
        }
    }
}
