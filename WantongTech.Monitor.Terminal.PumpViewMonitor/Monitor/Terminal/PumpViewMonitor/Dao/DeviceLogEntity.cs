using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WantongTech.Monitor.Terminal.PumpViewMonitor.Monitor.Terminal.PumpViewMonitor.Dao
{
    public class DeviceLogEntity
    {
        public string Id { get; set; }

        public string DeviceName { get; set; }

        public int DeviceState { get; set; }

        public string Description { get; set; }

        public DateTime GatherTime { get; set; }
    }
}
