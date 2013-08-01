using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace ChromelessWPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer _timer = new Timer();
        private double _yVel = 1f;
        private double _xVel = 1f;
        private const double Friction = 1f;
        private const double Gravity = .8f;
        private const double AirMax = 16f;
        private const double TerminalVelocity = 16f;
        private readonly Rectangle _area;
        private bool _isDragging;
        private Point _anchorPoint;
        private Vector _anchorDelta = new Vector(1f, 1f);

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
            if (_isDragging) return;
            
            _yVel += Gravity;
            if (_yVel > TerminalVelocity)
            {
                _yVel = TerminalVelocity;
            }

            if (_xVel > 0)
            {
                _xVel -= Friction;
            }

            if (_xVel < 0)
            {
                _xVel += Friction;
            }
            
            Top += _yVel;
            Left += _xVel;

            if (Top + Height >= _area.Bottom)
            {
                Top = _area.Bottom - Height;
                //bounce
                _yVel = -_yVel;
            }
        }

        private int count = 0;
        private Vector _anchorSum = new Vector();
        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                count++;
                Point currentPoint = PointToScreen(e.GetPosition(this));
                Left = Left + currentPoint.X - _anchorPoint.X;
                Top = Top + currentPoint.Y - _anchorPoint.Y;
                _anchorDelta = currentPoint - _anchorPoint;
                _anchorPoint = currentPoint;
                _anchorSum += _anchorDelta;
            }
        }

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            count = 0;
            Textout.Text = "";
            _isDragging = true;
            Mouse.Capture(this, CaptureMode.SubTree);
            _anchorPoint = PointToScreen(e.GetPosition(this));
        }

        private void Circle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var avDelta = _anchorSum/count;
            Textout.Text = count.ToString();
            _isDragging = false;
            ReleaseMouseCapture();

            //if (double.IsInfinity(avDelta.Y))
            //{
            //    _yVel = 0;
            //}
            //else
            //{
            //    _yVel = (float)avDelta.Y;
            //}

            //if (double.IsInfinity(avDelta.X))
            //{
            //    _xVel = 0;
            //}
            //else
            //{
            //    _xVel = (float)avDelta.X;
            //}
        }
    }
}
