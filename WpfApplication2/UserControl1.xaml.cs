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
using System.Windows.Threading;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
      private float _leftPitch = 0;
        private float _rightPitch = 0;
        private float canvasRadius = 170;   

        private float pitchLimit = 30; 

        public float leftPitch
        {
            get
            {
                return _leftPitch;
            }
            set
            {
                if(value >= -pitchLimit && value <= pitchLimit){
                    _leftPitch = value;
                }
                else if(value < -pitchLimit){
                    _leftPitch = -pitchLimit;
                }
                else{
                    _leftPitch = pitchLimit;
                }

                float calculatedValue = (_leftPitch / pitchLimit) * canvasRadius;
                Dispatcher.Invoke(()=>Canvas.SetTop(leftGroup,calculatedValue));
            }
        }

        public float rightPitch
        {
            get
            {
                return _rightPitch;
            }
            set
            {
                if (value >= -pitchLimit && value <= pitchLimit)
                {
                    _rightPitch = value;
                }
                else if (value < -pitchLimit)
                {
                    _rightPitch = -pitchLimit;
                }
                else
                {
                    _rightPitch = pitchLimit;
                }

                float calculatedValue = (_rightPitch / pitchLimit) * canvasRadius;
                Dispatcher.Invoke(() => Canvas.SetTop(rightGroup, calculatedValue));
            }
        }

        private float _Roll = 0;

        public float Roll
        {
            get
            {
                return _Roll;
            }
            set
            {
                _Roll = value;
                Dispatcher.Invoke(() => attitudeGrid.RenderTransform = new RotateTransform(_Roll, 190, 190));    
                Dispatcher.Invoke(() => rollLabel.Content = (int)_Roll);
            }
        }

        public UserControl1()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Roll = 0;
            leftPitch = 0;
            rightPitch = 0;
        }
        }

    }

