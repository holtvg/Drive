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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        private int _Heading = 0;
        public int Heading
        {
            get
            {
                return _Heading;
            }
            set
            {
                _Heading = value;
                Dispatcher.Invoke(() => digitsImage.RenderTransform = new RotateTransform(-_Heading));   
            }
        }

        public UserControl2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
