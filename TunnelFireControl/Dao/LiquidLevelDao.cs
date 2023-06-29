using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TunnelFireControl.Dao
{
    public class LiquidLevelDao
    {
        public void Save(LiquidLevelEntity entity)
        {
            string sql = string.Format("insert into liquid_level_log(id,device_name,liquid_level,description,gather_time) values('{0}','{1}','{2}','{3}','{4}')", entity.Id, entity.DeviceName, entity.LiquidLevel , entity.Description, entity.GatherTime);
            SqlHelper.Instance.ExecuteBySql(sql);
        }

        public List<LiquidLevelEntity> FindAll()
        {
            List<LiquidLevelEntity> liquidLevelEntities = new List<LiquidLevelEntity>();
            string sql = "select * from liquid_level_log order by gather_time";
            DataTable dt = SqlHelper.Instance.FindBySql(sql);
            if (dt == null)
            {
                return liquidLevelEntities;
            }
            foreach(DataRow dr in dt.Rows)
            {
                LiquidLevelEntity liquidLevelEntity = new LiquidLevelEntity();
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
                    liquidLevelEntity.LiquidLevel = float.Parse(dr[2].ToString());
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
        public List<LiquidLevelEntity> FindByDeviceIdAandGatherTime(string deviceName,DateTime startTime,DateTime endTime)
        {
            List<LiquidLevelEntity> liquidLevelEntities = new List<LiquidLevelEntity>();
            string sqlbase = "select * from liquid_level_log";
            string sql = string.Format("{0} where device_name='{1}' and gather_time BETWEEN '{2}' and '{3}' order by gather_time desc", sqlbase,deviceName,startTime,endTime);
            DataTable dt = SqlHelper.Instance.FindBySql(sql);
            if (dt == null)
            {
                return liquidLevelEntities;
            }
            foreach (DataRow dr in dt.Rows)
            {
                LiquidLevelEntity liquidLevelEntity = new LiquidLevelEntity();
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
                    liquidLevelEntity.LiquidLevel = float.Parse(dr[2].ToString());
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
