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
        String target = "50.28.225.39";
        int port = 3670;

        //BackgroundWorker joyStickThread = new BackgroundWorker();

        //DirectInput DI;
        //Joystick JS;
        //Guid joystickGUID;

        //  float xVal;

        logitechX3D testJoystick;
     //   hermesAnyConnect HAC = hermesAnyConnect.getInstance();
      //  Device testTeensy;


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

            
            irisClient = new IrisClient(100, IPAddress.Parse(target),port);
            irisClient.dataReceivedFromServer += IrisClient_dataReceivedFromServer;
            irisClient.connectionStatusChanged += testClient_connectionStatusChanged;

            //DI = new DirectInput();
            //joystickGUID = getGUID();

            //JS = new Joystick(DI, joystickGUID);
            //JS.Acquire();

            //joyStickThread.WorkerReportsProgress = true;
            //joyStickThread.WorkerSupportsCancellation = true;
            //joyStickThread.DoWork += new DoWorkEventHandler(monitorJoystick);
            //joyStickThread.ProgressChanged += joyStickThread_ProgressChanged;
            //joyStickThread.RunWorkerAsync();

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
          //  joyStickVisualizer.logitechX3DJoyStickSource = testJoystick;
         //   HAC.deviceListUpdated += HAC_deviceListUpdated;
         //   HAC.start();
        }

        //private void HAC_deviceListUpdated(bool connected, string comName)
        //{
        //    // MessageBox.Show("Conn: " + connected + " COM: " + comName);
        //    Console.WriteLine("Conn: " + connected + " COM: " + comName);
        //    if (connected && testTeensy == null)
        //    {
        //        testTeensy = HAC.DeviceList[comName];
        //        //Chatter_ConnectionStatusChanged(testTeensy.clientConnected);
        //        testTeensy.connectionStatusChanged += Chatter_ConnectionStatusChanged;
        //        testTeensy.messageReceived += Chatter_MessageReceived;
        //    }
        //}
        private void Chatter_MessageReceived(string ID, string data)
        {
            Console.WriteLine("REC-->\nID: " + ID + " Data: " + data + "\n");
        }

        //private void Chatter_ConnectionStatusChanged(bool connected)
        //{
        //    Console.WriteLine("HIT");
        //    if (connected)
        //    {
        //        Dispatcher.Invoke(() => connectedEllipse.Fill = Brushes.Green);
        //    }
        //    else
        //    {
        //        Dispatcher.Invoke(() => connectedEllipse.Fill = Brushes.Red);
        //    }
        //}


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

             //   if (testTeensy != null)
                
                    String toSend = xVal + "," + yVal + "," + zVal;
                    Console.WriteLine("Sending: " + toSend);
                  //  testTeensy.sendMessage("jxyz", toSend);
                    irisClient.sendData("jxyz", toSend);
                
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
            irisClient.sendData("jxyz", toSend);
            //   testTeensy.sendMessage("jxyz", toSend);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         //   HAC.Dispose();
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

                case "GPS_Loc":
                    //viewModel.RoverLatitude = double.Parse(data.Split(',')[0]);
                    //viewModel.RoverLongitude = double.Parse(data.Split(',')[1]);
                    //  mapDisplay.RoverLocation = new double[] { viewModel.RoverLatitude, viewModel.RoverLongitude };
                  //  mapDisplay.RoverLatitude = double.Parse(data.Split(',')[0]);
                    // mapDisplay.RoverLocation = new double[] { viewModel.RoverLatitude, viewModel.RoverLongitude };
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

                    break;

                case "PB_Current":
                    Console.WriteLine("Power Board Current: {0}", data);
                    break;

                default:
                    Console.WriteLine("Unknown command: {0}|{1}", ID, data);
                    break;
            }
        }


        //private Guid getGUID()
        //{
        //    Guid jsGuid = Guid.Empty;
        //    foreach (DeviceInstance deviceInstance in DI.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
        //    {
        //        jsGuid = deviceInstance.InstanceGuid;
        //        Console.WriteLine("Product Name: " + deviceInstance.ProductName);
        //    }

        //    // If Gamepad not found, look for a Joystick
        //    if (jsGuid == Guid.Empty)
        //        foreach (DeviceInstance deviceInstance in DI.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
        //        {
        //            jsGuid = deviceInstance.InstanceGuid;
        //            Console.WriteLine("Product Name: " + deviceInstance.ProductName);
        //        }

        //    // If Joystick not found, throws an error
        //    if (jsGuid == Guid.Empty)
        //    {
        //        MessageBox.Show("Unable to find a joystick...");
        //        Environment.Exit(0);
        //    }
        //    return jsGuid;
        //}

        //private void monitorJoystick(object sender, DoWorkEventArgs e)
        //{
        //    while (true)
        //    {
        //        joyStickThread.ReportProgress(1, JS.GetCurrentState()); //the number doesnt matter here...
        //        Thread.Sleep(50);
        //    }
        //}

        //void joyStickThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    JoystickState js = (JoystickState)e.UserState;
        //  //  Action updateGUI = delegate
        //    {
        //        //xValTextBox.Content = js.X;
        //        //yValTextBox.Content = js.Y;
        //        //twistValTextBox.Content = js.RotationZ; //Twist
        //        //leftThrotValTextBox.Content = js.Z; //WHY!?!?!?!
        //        //rightThrotValTextBox.Content = js.Sliders[0];

        //         xVal = js.X;
        //      //  string xval = Convert.ToString(xVal);
        //       // irisClient.sendData("xVal", xval);

        //        float yVal= js.Y;
        //        string yval = Convert.ToString(yVal);
        //     //   irisClient.sendData("yVal", yval);
        //        float twistVal = js.RotationZ; //Twist
        //        string twistval = Convert.ToString(twistVal);
        //      //  irisClient.sendData("twistVal", twistval);
        //        float leftThrotVal = js.Z; //WHY!?!?!?!
        //        string leftthrotval = Convert.ToString(leftThrotVal);
        //     //   irisClient.sendData("leftThrotVal", leftthrotval);
        //        float rightThrotVal = js.Sliders[0];
        //        string rightthrotval = Convert.ToString(rightThrotVal);
        //      //  irisClient.sendData("rightThrotVal", rightthrotval);

        //        //Console.WriteLine("hat: " + js.PointOfViewControllers[0]);
        //        string res = "";
        //        bool[] buttonRes = js.Buttons;
        //        for (int buttonIndex = 0; buttonIndex < buttonRes.Length; buttonIndex++)
        //        {
        //            if (buttonRes[buttonIndex])
        //            {
        //                res += " " + buttonIndex + " ";
        //            }
        //        }
        //        if (res != "")
        //        {
        //            Console.WriteLine(res);
        //          //  irisClient.sendData("button", res);
        //        }
        //    };
        // //   Dispatcher.Invoke(updateGUI);
        //}



         
        //public float changed
        //{
        //    get { return xVal; }
        //    set
        //    {
        //        xVal = value;
        //        if (xVal == 1)
        //        {
        //            string xval = Convert.ToString(xVal);
        //            irisClient.sendData("xVal", xval);
        //        }
        //    }
        //}


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{

        //}

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{

        //}

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
