using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TunnelFireControl.Dao
{
    public class LiquidLevelEntity
    {
        public string Id { get; set; }

        public string DeviceName { get; set; }

        public float LiquidLevel { get; set; }

        public string Description { get; set; }

        public DateTime GatherTime { get; set; }
    }
}
