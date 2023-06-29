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
using System.Windows.Shapes;

namespace TunnelFireControl
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private string _username;
        private string _password;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _username = System.Configuration.ConfigurationSettings.AppSettings["username"].ToString();
                _password = System.Configuration.ConfigurationSettings.AppSettings["password"].ToString();

                ClearData();
            }
            catch { }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            string userName = txtUserName.Text;
            string passWord = passwordBox.Password;
            if (userName == null || userName == "")
            {
                MessageBox.Show("请输入用户名");
                return;
            }
            if (userName.Equals(_username) && passWord.Equals(_password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.WindowState = WindowState.Maximized;
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.SetLogWindow(this);
                mainWindow.UserName = userName;
                mainWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("登录失败");
            }
        }

        private void ClearData()
        {
            string sql = string.Format("delete from liquid_level_log where gather_time<'{0}'",DateTime.Now.AddYears(-1));
            TunnelFireControl.Dao.SqlHelper.Instance.ExecuteBySql(sql);
        }
    }
}
