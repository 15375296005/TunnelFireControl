using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TunnelFireControl.Dao;
using WantongTech.Monitor.Terminal.PumpViewMonitor.Monitor.Terminal.PumpViewMonitor.Dao;

namespace WantongTech.Monitor.Terminal.PumpViewMonitor
{
    /// <summary>
    /// LiquidLevelDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PumpDataWindow : Window
    {
        public PumpDataWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxTunnelName.Items.Add("梅山湖隧道");
            cbxTunnelName.Items.Add("将军岭隧道");
            cbxTunnelName.Items.Add("槐树湾隧道");
            cbxTunnelName.Items.Add("李集1#隧道");
            cbxDeviceName.Items.Add("消防水泵1#");
            cbxDeviceName.Items.Add("消防水泵2#");

            for (int i = 0; i < 24; i++)
            {
                cbxStartHour.Items.Add(i.ToString().PadLeft(2, '0'));
                cbxEndHour.Items.Add(i.ToString().PadLeft(2, '0'));
            }

            for (int i = 0; i < 60; i++)
            {
                cbxStartMinute.Items.Add(i.ToString().PadLeft(2, '0'));
                cbxEndMinute.Items.Add(i.ToString().PadLeft(2, '0'));
            }
            cbxTunnelName.SelectedIndex = 0;
            cbxDeviceName.SelectedIndex = 0;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;

            cbxStartHour.SelectedIndex = hour - 1 > 0 ? hour - 1 : 0;
            cbxStartMinute.SelectedIndex = minute;
            cbxEndHour.SelectedIndex = hour;
            cbxEndMinute.SelectedIndex = minute;

            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now;

        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {

            var dlg = new SaveFileDialog();
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = false;
            dlg.Title = "请指定文件保存路径";
            dlg.Filter = "CSV Files(*.csv)|*.csv";
            dlg.DefaultExt = ".csv";

            if (true == dlg.ShowDialog())
            {

                using (var sw = new StreamWriter(dlg.OpenFile(), Encoding.Default))
                {
                    sw.WriteLine("序列号,设备名称,设备状态,描述,采集时间");
                    
                    if (lsvData.ItemsSource==null)
                    {
                        MessageBox.Show("当前数据为空");
                    }
                    else
                    {
                        foreach (var temp in lsvData.ItemsSource)
                        {
                            DeviceLogDto dto=temp as DeviceLogDto;
                            sw.WriteLine("{0},{1},{2},{3},{4}",dto.Id,dto.Name,dto.State,dto.Description,dto.GatherTime);

                        }
                    }
                    
                }
                MessageBox.Show("导出成功");
              
            }
        }


        private void btnQueryData_Click(object sender, RoutedEventArgs e)
        {
            //if (lsvData.Items.Count > 0)
            //{
            //    lsvData.Items.Clear();
            //}
            string deviceName = cbxTunnelName.Text + cbxDeviceName.Text;
            string sdate = dpStartDate.SelectedDate.HasValue ? dpStartDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd") : DateTime.Now.Date.ToString("yyyy-MM-dd");
            string edate = dpEndDate.SelectedDate.HasValue ? dpEndDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd") : DateTime.Now.Date.ToString("yyyy-MM-dd");
            string sHour = cbxStartHour.Text;
            string sMinute = cbxStartMinute.Text;
            string eHour = cbxEndHour.Text;
            string eMinute = cbxEndMinute.Text;
            string sdt = string.Format("{0} {1}:{2}:00", sdate, sHour, sMinute);
            string edt = string.Format("{0} {1}:{2}:00", edate, eHour, eMinute);
            DateTime startTime = DateTime.Parse(sdt);
            DateTime endTime = DateTime.Parse(edt);
            TimeSpan timeSpan = endTime - startTime;
            int day = (int)timeSpan.TotalHours;
            if (day > 5)
            {
                lsvData.ItemsSource = null;
                MessageBox.Show("查询时间不要超过5小时");
                return;
            }
            DeviceLogDao liquidLevelDao = DaoFactory.deviceLogDao;
            List<DeviceLogEntity> liquidLevelEntities = liquidLevelDao.FindByPumpDeviceIdAandGatherTime(deviceName, startTime, endTime);
            int sn = 0;
            if (liquidLevelEntities != null && liquidLevelEntities.Count > 0)
            {
                List<DeviceLogDto> liquidLevelDtos = new List<DeviceLogDto>();
                foreach (DeviceLogEntity entity in liquidLevelEntities)
                {
                    DeviceLogDto dto = new DeviceLogDto();
                    dto.Id = sn++;
                    dto.Name = entity.DeviceName;
                    switch (entity.DeviceState)
                    {
                        case 1:                           
                            dto.State = "开";
                            break;
                        case 0:
                            dto.State = "关";
                            break;
                    }                  
                    dto.Description = entity.Description;
                    dto.GatherTime = entity.GatherTime;
                    liquidLevelDtos.Add(dto);
                }
                lsvData.ItemsSource = liquidLevelDtos;
            }
            else
            {
                lsvData.ItemsSource = null;
            }
        }
    }
}
