using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WantongTech.Monitor.Terminal.PumpViewMonitor
{
    public class PumpViewMonitorView : IContentView
    {

        private FireControlMonitor controlMonitor;

        public PumpViewMonitorView(IUIApplication application)
        {
            controlMonitor = new FireControlMonitor();
        }

        #region IContentView 成员

        public string Caption
        {
            get { return "泵站监控"; }
        }

        public object Content
        {
            get { return controlMonitor; }
        }

        public void Load()
        {

        }

        public void UnLoad()
        {

        }

        #endregion
    }
}
