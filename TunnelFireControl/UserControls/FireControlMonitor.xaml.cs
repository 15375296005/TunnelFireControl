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
using System.Threading.Tasks;

namespace TunnelFireControl.UserControls
{
    /// <summary>
    /// FireControlMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class FireControlMonitor : UserControl
    {
        private FireControlMain curFireControl;
        private FireControlMain mshFireControl;
        private FireControlMain jjlFireControl;
        private FireControlMain hswFireControl;
        private FireControlMain ljFireControl;
        public FireControlMonitor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            Init();
            if (curFireControl != null)
            {
                grid_main.Children.Remove(curFireControl);
            }
            curFireControl = mshFireControl;
            grid_main.Children.Add(curFireControl);
        }

        private void Init()
        {
            if (mshFireControl == null)
            {
                mshFireControl = new FireControlMain();
                mshFireControl.TunnelName = "梅山湖隧道";
            }
            if (jjlFireControl == null)
            {
                jjlFireControl = new FireControlMain();
                jjlFireControl.TunnelName = "将军岭隧道";
            }
            if (hswFireControl == null)
            {
                hswFireControl = new FireControlMain();             
                hswFireControl.TunnelName = "槐树湾隧道";
            }
            if (ljFireControl == null)
            {
                ljFireControl = new FireControlMain();               
                ljFireControl.TunnelName = "李集1#隧道";
            }
            Task task = Task.Factory.StartNew(() => 
            {
                try
                {
                    string commu0 = "2:172.29.56.38:10123";
                    mshFireControl.Init();
                    mshFireControl.InitPlcDevice(commu0);
                    string commu1 = "2:172.29.56.39:10123";
                    jjlFireControl.Init();
                    jjlFireControl.InitPlcDevice(commu1);
                    string commu2 = "2:172.29.56.40:10123";
                    hswFireControl.Init();
                    hswFireControl.InitPlcDevice(commu2);
                    string commu3 = "2:172.29.56.41:10123";
                    ljFireControl.Init();
                    ljFireControl.InitPlcDevice(commu3);
                }
                catch { }
            });
        }

        private void btnMeiShanHu_Click(object sender, RoutedEventArgs e)
        {
            if (mshFireControl == null)
            {
                string commu = "2:172.29.56.38:10123";
                mshFireControl = new FireControlMain();
                mshFireControl.Init();
                mshFireControl.InitPlcDevice(commu);
                mshFireControl.TunnelName = "梅山湖隧道";
            }
            if (curFireControl != null)
            {
                grid_main.Children.Remove(curFireControl);
            }
            curFireControl = mshFireControl;
            grid_main.Children.Add(curFireControl);
        }

        private void btnJiangJunLing_Click(object sender, RoutedEventArgs e)
        {
            if (jjlFireControl == null)
            {
                string commu = "2:172.29.56.39:10123";
                jjlFireControl = new FireControlMain();
                jjlFireControl.Init();
                jjlFireControl.InitPlcDevice(commu);
                jjlFireControl.TunnelName = "将军岭隧道";
            }
            if (curFireControl != null)
            {
                grid_main.Children.Remove(curFireControl);
            }
            curFireControl = jjlFireControl;
            grid_main.Children.Add(curFireControl);
        }

        private void btnHuaiShuWan_Click(object sender, RoutedEventArgs e)
        {
            if (hswFireControl == null)
            {
                string commu = "2:172.29.56.40:10123";
                hswFireControl = new FireControlMain();
                hswFireControl.Init();
                hswFireControl.InitPlcDevice(commu);
                hswFireControl.TunnelName = "槐树湾隧道";
            }
            if (curFireControl != null)
            {
                grid_main.Children.Remove(curFireControl);
            }
            curFireControl = hswFireControl;
            grid_main.Children.Add(curFireControl);
        }

        private void btnLiJiOne_Click(object sender, RoutedEventArgs e)
        {
            if (ljFireControl == null)
            {
                string commu = "2:172.29.56.41:10123";
                ljFireControl = new FireControlMain();
                ljFireControl.Init();
                ljFireControl.InitPlcDevice(commu);
                ljFireControl.TunnelName = "李集1#隧道";
            }
            if (curFireControl != null)
            {
                grid_main.Children.Remove(curFireControl);
            }
            curFireControl = ljFireControl;
            grid_main.Children.Add(curFireControl);
        }

        private void btnQueryData_Click(object sender, RoutedEventArgs e)
        {
            LiquidLevelDataWindow liquidLevelDataWindow = new LiquidLevelDataWindow();
            liquidLevelDataWindow.ShowDialog();
        }
    }
}
