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
using TunnelFireControl.PlcDriver;
using TunnelFireControl.Dao;

namespace TunnelFireControl.UserControls
{
    public delegate void RefreshDeviceState();
    /// <summary>
    /// FireControlMain.xaml 的交互逻辑
    /// </summary>
    public partial class FireControlMain : UserControl
    {
        private System.Timers.Timer timer;

        private System.Timers.Timer timerGather;
        private IPlcDriver plcDriver;
        private RefreshDeviceState RefreshDeviceStateHandle;
        private RefreshDeviceState RefreshGatherHandle;
        private int lastPump1_state = 0;
        private int lastPump2_state = 0;
        private int sn = 1;
        private string tunnelName;
        public FireControlMain()
        {
            InitializeComponent();
            RefreshDeviceStateHandle = RefreshDevice;
            RefreshGatherHandle = RefreshLiquid;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public string TunnelName
        {
            get { return tunnelName; }
            set 
            { 
                tunnelName = value;
                lblTunnelName.Content = value;
            }
        }

        public void Init()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 5000;
                timer.Start();
            }

            if (timerGather == null)
            {
                timerGather = new System.Timers.Timer();
                timerGather.Elapsed += TimerGather_Elapsed;
                timerGather.Interval = 60000;
                timerGather.Start();
            }
        }

        public void InitPlcDevice(string communiction)
        {
            plcDriver = new LuanbeiSiemensDriver();
            plcDriver.Init(communiction);
        }

        private void TimerGather_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(RefreshGatherHandle);
            }
            catch { }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(RefreshDeviceStateHandle);
            }
            catch { }
        }

        private void RefreshLiquid()
        {
            float liquid1 = 0.0f;
            float liquid2 = 0.0f;
            if (plcDriver != null)
            {
                liquid1 = plcDriver.Read(0) / 10.0f;
                liquid2 = plcDriver.Read(1) / 10.0f;

                LiquidLevelDto liquidLevelDto1 = new LiquidLevelDto();
                liquidLevelDto1.Id = sn++;
                liquidLevelDto1.Name = TunnelName + "低水池液位";
                liquidLevelDto1.LiquidLevel = liquid1;
                liquidLevelDto1.Description = "正常";
                liquidLevelDto1.GatherTime = DateTime.Now;

                LiquidLevelDto liquidLevelDto2 = new LiquidLevelDto();
                liquidLevelDto2.Id = sn++;
                liquidLevelDto2.Name = TunnelName + "高水池液位";
                liquidLevelDto2.LiquidLevel = liquid2;
                liquidLevelDto2.Description = "正常";
                liquidLevelDto2.GatherTime = DateTime.Now;

                dsc_level.SetLevelValue(liquid1);
                gsc_level.SetLevelValue(liquid2);

                int count = lsvLiquid.Items.Count;
                if (count > 10)
                {
                    lsvLiquid.Items.RemoveAt(count - 1);
                    lsvLiquid.Items.RemoveAt(count - 2);
                }

                lsvLiquid.Items.Insert(0,liquidLevelDto1);
                lsvLiquid.Items.Insert(0,liquidLevelDto2);

                LiquidLevelEntity liquidLevelEntity1 = new LiquidLevelEntity();
                liquidLevelEntity1.Id = Guid.NewGuid().ToString();
                liquidLevelEntity1.DeviceName= TunnelName + "低水池液位";
                liquidLevelEntity1.LiquidLevel = liquid1;
                liquidLevelEntity1.Description= "正常";
                liquidLevelEntity1.GatherTime = DateTime.Now;

                LiquidLevelEntity liquidLevelEntity2 = new LiquidLevelEntity();
                liquidLevelEntity2.Id = Guid.NewGuid().ToString();
                liquidLevelEntity2.DeviceName = TunnelName + "高水池液位";
                liquidLevelEntity2.LiquidLevel = liquid2;
                liquidLevelEntity2.Description = "正常";
                liquidLevelEntity2.GatherTime = DateTime.Now;

                LiquidLevelDao liquidLevelDao = DaoFactory.liquidLevelDao;
                liquidLevelDao.Save(liquidLevelEntity1);
                liquidLevelDao.Save(liquidLevelEntity2);
                List<LiquidLevelEntity> liquidLevelEntities = liquidLevelDao.FindAll();
            }
        }

        private void RefreshDevice()
        {
            int state1 = 0;
            int state2 = 0;
            if (plcDriver != null)
            {
                state1 = plcDriver.Read(8);
                state2 = plcDriver.Read(9);
            }
            if (lastPump1_state != state1)
            {
                pump1.SetMode(1);
                pump1.SetState(state1);
                if (state1 == 1) 
                {
                    lbRunningLog.Items.Add(string.Format("水泵1#开启,开启时间为{0}",DateTime.Now));
                }
                else
                {
                    lbRunningLog.Items.Add(string.Format("水泵1#关闭,关闭时间为{0}", DateTime.Now));
                }
                lastPump1_state = state1;
            }
            if (lastPump2_state != state2)
            {
                pump2.SetMode(1);
                pump2.SetState(state2);
                if (state2 == 1)
                {
                    lbRunningLog.Items.Add(string.Format("水泵2#开启,开启时间为{0}", DateTime.Now));
                }
                else if (state2 == 0)
                {
                    lbRunningLog.Items.Add(string.Format("水泵2#关闭,关闭时间为{0}", DateTime.Now));
                }
                lastPump2_state = state2;
            }
        }
    }

    public class LiquidLevelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public float LiquidLevel { get; set; }

        public string Description { get; set; }

        public DateTime GatherTime { get; set; }
    }
}
