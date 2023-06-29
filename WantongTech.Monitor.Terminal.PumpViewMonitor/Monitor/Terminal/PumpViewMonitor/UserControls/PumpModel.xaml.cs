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

namespace WantongTech.Monitor.Terminal.PumpViewMonitor
{
    /// <summary>
    /// PumpModel.xaml 的交互逻辑
    /// </summary>
    public partial class PumpModel : UserControl
    {
        private string name;
        private BitmapImage pump_opened = new BitmapImage(new Uri("/TunnelFireControl;Component/Resource/pump_opened.png", UriKind.Relative));
        private BitmapImage pump_closed = new BitmapImage(new Uri("/TunnelFireControl;Component/Resource/pump_closed.png", UriKind.Relative));
        public PumpModel()
        {
            InitializeComponent();
        }
        public string DeviceName
        {
            get { return name; }
            set
            {
                name = value;
                lblName.Content = name;
            }
        }
        /// <summary>
        /// 设备界面设备状态
        /// </summary>
        /// <param name="state"></param>
        public void SetState(int state)
        {
            if (state == 1)
            {
                pump_image.Source = pump_opened;
            }
            else
            {
                pump_image.Source = pump_closed;
            }
        }

        /// <summary>
        /// 设置界面设备的远程/就地模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetMode(int mode)
        {
            //if (mode == 1)
            //{
            //    Brush brush = new SolidColorBrush(Colors.Yellow);
            //    elp_mode.Fill = brush;
            //    lblControlMode.Content = "远程";
            //}
            //else
            //{
            //    Brush brush = new SolidColorBrush(Colors.White);
            //    elp_mode.Fill = brush;
            //    lblControlMode.Content = "就地";
            //}
        }
    }
}
