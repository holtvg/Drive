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
using IrisSS;
using System.Net;
using SharpDX.DirectInput;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IrisClient irisClient;
        String target = "127.0.0.1";
        int port = 3670;

        public MainWindow()
        {
            InitializeComponent();

            
            irisClient = new IrisClient(300, IPAddress.Parse(target),port);
            irisClient.dataReceivedFromServer += IrisClient_dataReceivedFromServer;
            irisClient.connectionStatusChanged += testClient_connectionStatusChanged;

            
            //var directInput = new DirectInput();

            
            //var joystickGuid = Guid.Empty;

            //foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
            //            DeviceEnumerationFlags.AllDevices))
            //    joystickGuid = deviceInstance.InstanceGuid;

            
            //if (joystickGuid == Guid.Empty)
            //    foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
            //            DeviceEnumerationFlags.AllDevices))
            //        joystickGuid = deviceInstance.InstanceGuid;

            
            //if (joystickGuid == Guid.Empty)
            //{
            //    Console.WriteLine("No joystick/Gamepad found.");
            //    Console.ReadKey();
            //    Environment.Exit(1);
            //}

            
            //var joystick = new Joystick(directInput, joystickGuid);

            //Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);

            
            //var allEffects = joystick.GetEffects();
            //foreach (var effectInfo in allEffects)
            //    Console.WriteLine("Effect available {0}", effectInfo.Name);

            
            //joystick.Properties.BufferSize = 128;

            
            //joystick.Acquire();

            
            //while (true)
            //{
            //    joystick.Poll();
            //    var datas = joystick.GetBufferedData();
            //    foreach (var state in datas)
            //        Console.WriteLine(state);
            //}
        }

        private void testClient_connectionStatusChanged(bool connected)
        {
            if (connected)
            {
                Dispatcher.Invoke(() => connectionindicator.Fill = new SolidColorBrush(Colors.Green));
                Dispatcher.Invoke(() => pitchLabel_Copy4.Content = target);
                Dispatcher.Invoke(() => yawLabel_Copy1.Content = port);
            }
            else
            {
                Dispatcher.Invoke(() => connectionindicator.Fill = new SolidColorBrush(Colors.Red));
            }
        }

        private void IrisClient_dataReceivedFromServer(string ID, string data)
        {
            switch (ID)
            {
                case "heading":

                    headingInd.Heading = Int32.Parse(data);

                    Dispatcher.Invoke(() => pitchLabel_Copy3.Content = Math.Abs(Int32.Parse(data)));
                    break;
                case "leftPitch":

                    attitudeInd.leftPitch = float.Parse(data);
                    break;
                case "rightPitch":
 
                    attitudeInd.rightPitch = float.Parse(data);
                    break;
                case "roll":

                    attitudeInd.Roll = float.Parse(data);
                    break;
            }
        }







         

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void videoToggle_Click(object sender, RoutedEventArgs e)
        {
            Window1 subWindow = new Window1();
            subWindow.Show();
        }

        private void textBox_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
