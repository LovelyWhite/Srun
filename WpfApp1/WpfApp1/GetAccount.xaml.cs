using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class GetAccount : Window
    {
        public void DMouseMove(Object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Mouse.GetPosition(min).X < 800 && Mouse.GetPosition(min).Y < 28)
            {
                this.DragMove();
            }
        }
        public GetAccount()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {

            WindowState = WindowState.Minimized;
        }
    }
}
