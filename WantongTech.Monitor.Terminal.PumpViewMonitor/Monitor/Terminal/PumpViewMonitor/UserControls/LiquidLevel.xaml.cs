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
    /// LiquidLevel.xaml 的交互逻辑
    /// </summary>
    public partial class LiquidLevel : UserControl
    {
        private string name = "";
        public LiquidLevel()
        {
            InitializeComponent();
        }

        public void SetLevelValue(float value)
        {
            this.lblLiquidValue.Content = value.ToString();//.PadLeft(4,'0')
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
    }
}
