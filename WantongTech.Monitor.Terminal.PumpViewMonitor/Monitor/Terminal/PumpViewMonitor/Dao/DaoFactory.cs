using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WantongTech.Monitor.Terminal.PumpViewMonitor.Monitor.Terminal.PumpViewMonitor.Dao;

namespace TunnelFireControl.Dao
{
    public static class DaoFactory
    {
        public static LiquidLevelDao liquidLevelDao = new LiquidLevelDao();

        public static DeviceLogDao deviceLogDao = new DeviceLogDao();
    }
}
