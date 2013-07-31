using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ChromelessWPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer _timer = new Timer();
        private float _yVel = 1f;
        private const float Gravity = .8f;
        private const float TerminalVelocity = 16f;
        private readonly Rectangle _area;

        public MainWindow()
        {
            _timer.Interval = 1;
            _timer.Tick += _timer_Tick;
            InitializeComponent();
            _timer.Start();

            _area = Screen.PrimaryScreen.WorkingArea;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _yVel += Gravity;
            if (_yVel > TerminalVelocity)
            {
                _yVel = TerminalVelocity;
            }
            
            Top += _yVel;

            if (Top + Height >= _area.Bottom)
            {
                Top = _area.Bottom - Height;
                //bounce
                _yVel = -_yVel;
            }

        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _yVel = 1;
            DragMove();
        }
    }
}
