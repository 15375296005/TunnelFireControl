using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace WantongTech.Monitor.Terminal.PumpViewMonitor
{
    public class PumpViewMonitorPlugin : IPlugin
    {

        private IUIApplication application;
        private PumpViewMonitorView view;

        #region
        public void Load(IUIApplication application)
        {
            StreamResourceInfo sr = Application.GetResourceStream(
              new Uri("/WantongTech.Monitor.Terminal.Resource;Component/Icon/Menu/WeatherMonitor.png", UriKind.Relative));
            this.application = application;
            application.MainWindow.MainMenu.Items.AddButton("泵站监控", sr.Stream ,new EventHandler(MenuButton_Click));
            view = new PumpViewMonitorView(application);
            application.MainWindow.Workspace.RegisterView("PumpViewMonitor", view);
        }

        /// <summary>
        /// 卸载插件。
        /// </summary>
        public void UnLoad()
        {
        }
        #endregion

        #region Private Methods

        private void MenuButton_Click(object sender, EventArgs e)
        {
            application.MainWindow.Workspace.SwitchViewToScreen("PumpViewMonitor");
        }

        #endregion
    }
}
