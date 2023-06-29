using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using TunnelFireControl.Dao;

namespace WantongTech.Monitor.Terminal.PumpViewMonitor.Monitor.Terminal.PumpViewMonitor.Dao
{
    public class DeviceLogDao
    {
        public void Save(DeviceLogEntity entity)
        {
            string sql = string.Format("insert into device_state_log(id,device_name,state,description,gather_time) values('{0}','{1}','{2}','{3}','{4}')", entity.Id, entity.DeviceName, entity.DeviceState, entity.Description, entity.GatherTime);
            SqlHelper.Instance.ExecuteBySql(sql);
        }

        public List<DeviceLogEntity> FindByPumpDeviceIdAandGatherTime(string deviceName, DateTime startTime, DateTime endTime)
        {
            List<DeviceLogEntity> liquidLevelEntities = new List<DeviceLogEntity>();
            string sqlbase = "select * from device_state_log";
            string sql = string.Format("{0} where device_name='{1}' and gather_time BETWEEN '{2}' and '{3}' order by gather_time desc", sqlbase, deviceName, startTime, endTime);
           // MessageBox.Show(sql);
            DataTable dt = SqlHelper.Instance.FindBySql(sql);
            if (dt == null)
            {
                return liquidLevelEntities;
            }
            foreach (DataRow dr in dt.Rows)
            {
                DeviceLogEntity liquidLevelEntity = new DeviceLogEntity();
                if (dr[0] != null)
                {
                    liquidLevelEntity.Id = dr[0].ToString();
                }
                if (dr[1] != null)
                {
                    liquidLevelEntity.DeviceName = dr[1].ToString();
                }
                if (dr[2] != null)
                {
                    //MessageBox.Show(dr[2].ToString());    
                    liquidLevelEntity.DeviceState = int.Parse(dr[2].ToString());
                }
                if (dr[3] != null)
                {
                    liquidLevelEntity.Description = dr[3].ToString();
                }
                if (dr[4] != null)
                {
                    liquidLevelEntity.GatherTime = DateTime.Parse(dr[4].ToString());
                }
                liquidLevelEntities.Add(liquidLevelEntity);
            }
            return liquidLevelEntities;
        }
    }
}
