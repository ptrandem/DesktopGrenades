using System;
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
        private float _yVel = 1f;
        private const float Gravity = .8f;
        private const float TerminalVelocity = 16f;
        private readonly Rectangle _area;
        private bool _isDragging = false;
        private Point _anchorPoint;

        public MainWindow()
        {
            _timer.Interval = 1;
            _timer.Tick += _timer_Tick;
            InitializeComponent();
            _timer.Start();

            _area = Screen.PrimaryScreen.WorkingArea;
            Mouse.AddMouseUpHandler(this, (sender, args) => ReleaseMouseCapture());
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_isDragging) return;
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

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            Mouse.Capture(this, CaptureMode.SubTree);
            _anchorPoint = PointToScreen(e.GetPosition(this));
        }

        private void Ellipse_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            ReleaseMouseCapture();
            _yVel = 1f;
            
        }
        
        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = PointToScreen(e.GetPosition(this));
                Left = Left + currentPoint.X - _anchorPoint.X;
                Top = Top + currentPoint.Y - _anchorPoint.Y;
                _anchorPoint = currentPoint;
            }
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void Circle_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }
    }
}
