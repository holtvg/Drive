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
//using SharpDX.DirectInput;
using System.ComponentModel;
using System.Threading;
using logitechX3D_Lib;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IrisClient irisClient;
        //  String target = "127.0.0.1";
        //  String target = "155.99.9.29";
        // String target = "50.28.225.39";
         String target = "192.168.1.151";
       // String target = "155.99.10.112";
        int port = 3670;
        private double RoverLatitude = 0.0;
        private double RoverLongitude = 0.0;


        logitechX3D testJoystick;



        readonly double xRange = 500;
        readonly double yRange = 1300;
        readonly double zRange = 500;

        double throttlePer = 0;
        double xPer = 0;
        double yPer = 0;
        double zPer = 0;

        int xVal = 0;
        int yVal = 0;
        int zVal = 0;

        object lockObject = 0;


        public MainWindow()
        {
            InitializeComponent();
            axisCameraStream.OpenFeed();

            irisClient = new IrisClient(100, IPAddress.Parse(target),port);
            irisClient.dataReceivedFromServer += IrisClient_dataReceivedFromServer;
            irisClient.connectionStatusChanged += testClient_connectionStatusChanged;


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool worked;
            testJoystick = logitechX3D.getInstance(out worked);
            if (!worked)
            {
                MessageBox.Show("Couldnt find any joysticks...");
                return;
            }
            testJoystick.throttleChanged += testJoystick_throttleChanged;
            testJoystick.axisInputChanged += testJoystick_axisInputChanged;

          //  axisCameraStream.OpenFeed();
        }


        private void Chatter_MessageReceived(string ID, string data)
        {
            Console.WriteLine("REC-->\nID: " + ID + " Data: " + data + "\n");
        }



        private void testJoystick_axisInputChanged(logitechX3D.axisID ID, double percentage)
        {

            lock (lockObject)
            {
                //Console.WriteLine(ID + " set to: " + percentage);
                switch (ID)
                {
                    case logitechX3D.axisID.X:
                        if (Math.Abs(percentage - xPer) < .05)
                        {
                            return;
                        }
                        xPer = percentage * xRange;
                        xVal = (int)(xPer * throttlePer);
                        break;
                    case logitechX3D.axisID.Y:
                        if (Math.Abs(percentage - yPer) < .05)
                        {
                            return;
                        }
                        yPer = percentage * yRange;
                        yVal = (int)(yPer * throttlePer);
                        break;
                    case logitechX3D.axisID.Twist:
                        if (Math.Abs(percentage - zPer) < .05)
                        {
                            return;
                        }
                        zPer = percentage * zRange;
                        zVal = (int)(zPer * throttlePer);
                        break;
                }

             
                
                    String toSend = xVal + "," + yVal + "," + zVal;
                    Console.WriteLine("Sending: " + toSend);
                  
                    irisClient.sendData("JXYZ", toSend);
                
            }
        }


        void testJoystick_throttleChanged(double percentage)
        {
            Console.WriteLine("Throttle at: " + percentage);
            throttlePer = percentage;
            xVal = (int)(xPer * throttlePer);
            yVal = (int)(yPer * throttlePer);
            zVal = (int)(zPer * throttlePer);
            String toSend = xVal + "," + yVal + "," + zVal;
            Console.WriteLine(toSend);
            irisClient.sendData("JXYZ", toSend);
            //   testTeensy.sendMessage("jxyz", toSend);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         //   axisCameraStream.CloseFeed();
            
        }

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    axisCameraStream.CloseFeed();
        
        //}



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

                case "GPS_Loc":
                    //viewModel.RoverLatitude = double.Parse(data.Split(',')[0]);
                    //viewModel.RoverLongitude = double.Parse(data.Split(',')[1]);
                    //  mapDisplay.RoverLocation = new double[] { viewModel.RoverLatitude, viewModel.RoverLongitude };
                    RoverLongitude = double.Parse(data.Split(',')[1]);
                    RoverLatitude = double.Parse(data.Split(',')[0]);
                    Dispatcher.Invoke(() => mapDisplay.RoverLocation = new double[] { RoverLatitude, RoverLongitude });
                    Dispatcher.Invoke(() => mapDisplay.Center = new double[] { RoverLatitude, RoverLongitude });
                    Dispatcher.Invoke(() => pitchLabel_Copy1.Content = RoverLatitude);
                    Dispatcher.Invoke(() => pitchLabel_Copy2.Content = RoverLongitude);
                    break;

                case "ATT_L":
                    //viewModel.Yaw = double.Parse(data.Split(',')[0]);
                    //viewModel.LeftPitch = double.Parse(data.Split(',')[1]);
                    //viewModel.RightPitch = double.Parse(data.Split(',')[1]);
                    //viewModel.Roll = double.Parse(data.Split(',')[2]);
                  //  attitudeInd.Yaw = float.Parse(data.Split(',')[0]);
                    attitudeInd.leftPitch = float.Parse(data.Split(',')[1]);

                    attitudeInd.rightPitch = float.Parse(data.Split(',')[1]);

                    attitudeInd.Roll = float.Parse(data.Split(',')[2]);

                    attitudeInd.Yaw = float.Parse(data.Split(',')[0]);

                    break;

                case "PB_Current":
                    Console.WriteLine("Power Board Current: {0}", data);
                    break;

                case "VS_C":
                    //if (Terminal.Drive != (Terminal)int.Parse(data.Split(',')[0])) break;
                    //axisCameraStream.Camera = (AxisCamera)int.Parse(data.Split(',')[1]);
                    //axisCameraStream.Resolution = (AxisResolution)int.Parse(data.Split(',')[2]);
                    //axisCameraStream.FPS = int.Parse(data.Split(',')[3]);
                    //axisCameraStream.Compression = int.Parse(data.Split(',')[4]);
                    //axisCameraStream.CloseFeed();
                    //axisCameraStream.OpenFeed();
                    break;
                case "VS_R":

                case "DLOCK":
                    bool locked = bool.Parse(data);
                    if (locked){
                        //   engage = true;
                        Dispatcher.Invoke(() => connectionindicator_Copy.Fill = new SolidColorBrush(Colors.Green));
                    }
                    else {
                        //   engage = false;
                        Dispatcher.Invoke(() => connectionindicator_Copy.Fill = new SolidColorBrush(Colors.Red));
                    }
                    break;
                default:
                    Console.WriteLine("Unknown command: {0}|{1}", ID, data);
                    break;
            }
        }





        private void Window_KeyDown(object sender, KeyEventArgs e)
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
