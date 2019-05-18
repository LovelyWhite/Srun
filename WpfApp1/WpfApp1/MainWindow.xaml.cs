using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        Data d = new Data();
        public MainWindow()
        {
            InitializeComponent();
            var pReadByte = new byte[3000];
            FileStream myStream = new FileStream("data.dat", FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader myReader = new BinaryReader(myStream);
            pReadByte = myReader.ReadBytes(3000);
            using (MemoryStream ms = new MemoryStream(pReadByte))
            {
                IFormatter bf = new BinaryFormatter();
                if (ms.Length != 0)
                {
                    d = (Data)bf.Deserialize(ms);
                    Account.Text = d.x;
                    Password.Password = d.x1;
                    Reme.IsChecked = true;
                }
            }
            InitData();

        }

        public void InitData()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://172.16.154.130/cgi-bin/rad_user_info");
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var x = responseString.Split(new char[] { ',' });
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(x[1] + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            TimeSpan time = new TimeSpan(long.Parse(x[2] + "0000000") - long.Parse(x[1] + "0000000"));
            alart.Text = "学号：" + x[0] + '\n' + "登录时间：" + dtStart.Add(toNow).ToString() + '\n';
        }
        public void DMouseMove(Object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Mouse.GetPosition(min).X < 800 && Mouse.GetPosition(min).Y < 28)
            {
                this.DragMove();
            }
        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Prog.Visibility = Visibility.Visible;
            Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(() =>
              {
                  Prog.Value = 50;
                  d = new Data(Account.Text, Password.Password, "login", "1");
                  var request = (HttpWebRequest)WebRequest.Create(d.GetUrl());
                  request.Method = "POST";
                  request.ContentType = "application/x-www-form-urlencoded";
                  var data = Encoding.ASCII.GetBytes(d.GetPram());
                  Prog.Value = 70;
                  request.ContentLength = data.Length;
                  try
                  {
                      Prog.Value = 80;
                      using (var stream = request.GetRequestStream())
                      {
                          stream.Write(data, 0, data.Length);
                          
                      }
                      var response = request.GetResponse();
                      var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                      Prog.Value = 90;
                      ShowAlart(responseString);

                  }
                  catch (Exception a)
                  {
                      alart.Text = a.Message;
                  }
                  Prog.Value = 100;
                  Prog.Visibility = Visibility.Hidden;
              }));
        }
        private void ShowAlart(string content)
        {
            if (content.Contains("login_ok"))
            {
                alart.Text = "登录成功";
                var pReadByte = new byte[3000];
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, d);
                    pReadByte = ms.GetBuffer();
                }

                FileStream myStream = new FileStream("data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryWriter myWriter = new BinaryWriter(myStream);
                myWriter.Write(pReadByte);
            }
            else if (content.Contains("ip_already"))
            {
                MessageBox.Show("当前IP已有账号登录", "提示", MessageBoxButton.OK);
            }
            else if (content.Contains("Password is error") || content.Contains("password_algo_error"))
            {
                MessageBox.Show("密码错误", "提示", MessageBoxButton.OK);
            }
            else if (content.Contains("logout_ok"))
            {
                MessageBox.Show("登出成功", "提示", MessageBoxButton.OK);
               
            }
            else if (content.Contains("User not found"))
            {
                MessageBox.Show("账号不存在", "提示", MessageBoxButton.OK);

            }
            else if (content.Contains("LOGOUT failed"))
            {
                MessageBox.Show("登出失败", "提示", MessageBoxButton.OK);
            }
            else if (content.Contains("Limit Users Err"))
            {
                MessageBox.Show("该账号已在其他地方登录", "提示", MessageBoxButton.OK);
            }
            else if (content.Contains("You are not online"))
            {
                MessageBox.Show("你不在线", "提示", MessageBoxButton.OK);
            }
            else if (content.Contains("missing_required_parameters_error"))
            {
                MessageBox.Show("请输入用户名和密码", "提示", MessageBoxButton.OK);
            }
            else if (content.Contains("Status_Err"))
            {
                MessageBox.Show("状态错误(可能是欠费)", "提示", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(content, "未知错误", MessageBoxButton.OK);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Window window = new GetAccount();
            window.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", " http://172.16.154.130:8800/home");
        }
    }

    [Serializable]
    class Data
    {
        public String x;
        public String x1;
        private String URL;
        private String action;
        private String userName;
        private String passWord;
        private String type;
        private String n;
        private String ac_id;

        public Data()
        {

        }
        public Data(String userName, String passWord, String action, String ac_id)
        {
            x = userName;
            x1 = passWord;
            URL = "http://172.16.154.130:69/cgi-bin/srun_portal";
            this.userName = "username=%7bSRUN3%7d%0d%0a" + UserEncode(userName);
            this.passWord = "&password=" + passWord;
            this.ac_id = "&ac_id=" + ac_id;
            this.action = "&action=" + action;
            n = "&n=117";
            type = "&type=11";
        }

        public string GetUrl()
        {
            return URL;
        }
        public  string GetPram()
        {
            return userName += passWord += action += type += n += ac_id;
        }

        private string UserEncode(string userName)
        {
            char[] r = userName.ToArray<char>();
            for (int i = 0; i < r.Length; i++)
                r[i] += (char)4;
            return new string(r);
        }
    }

}
