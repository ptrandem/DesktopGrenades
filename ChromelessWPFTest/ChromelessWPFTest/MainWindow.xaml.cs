using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
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

        private World _world;
        private Body _circleBody;
        private Body _groundBody;
        private Body _groundBody2;

        public MainWindow()
        {
            _timer.Interval = 1;
            _timer.Tick += _timer_Tick;
            InitializeComponent();
            _timer.Start();
            _area = Screen.PrimaryScreen.WorkingArea;

            _world = new World(new Vector2(0, 9.8f));

            // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(new Vector2((float)_area.Width/2, (float)_area.Height/2));

            Left = (float)_area.Width/2 - (Width/2);
            Top = (float) _area.Height/2 - (Height/2);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(100 / 2f), 10f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.ApplyTorque(100f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(_area.Width * 2), ConvertUnits.ToSimUnits(1f), 1f, ConvertUnits.ToSimUnits(new Vector2(0, _area.Height-50)));
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.8f;
            _groundBody.Friction = 0.5f;

            // Create east wall
            _groundBody2 = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(1f), ConvertUnits.ToSimUnits(_area.Height * 2), 1f, ConvertUnits.ToSimUnits(new Vector2(_area.Width-50, _area.Height)));
            _groundBody2.IsStatic = true;
            _groundBody2.Restitution = 0.8f;
            _groundBody2.Friction = 0.5f;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _world.Step(.025f);
            Top = ConvertUnits.ToDisplayUnits(_circleBody.Position.Y);
            Left = ConvertUnits.ToDisplayUnits(_circleBody.Position.X);

            if (_isDragging) return;
            
            //_yVel += Gravity;
            //if (_yVel > TerminalVelocity)
            //{
            //    _yVel = TerminalVelocity;
            //}

            //if (_xVel > 0)
            //{
            //    _xVel -= Friction;
            //}

            //if (_xVel < 0)
            //{
            //    _xVel += Friction;
            //}
            
            //Top += _yVel;
            //Left += _xVel;

            //if (Top + Height >= _area.Bottom)
            //{
            //    Top = _area.Bottom - Height;
            //    //bounce
            //    _yVel = -_yVel;
            //}
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = PointToScreen(e.GetPosition(this));
                Left = Left + currentPoint.X - _anchorPoint.X;
                Top = Top + currentPoint.Y - _anchorPoint.Y;
                _anchorDelta = currentPoint - _anchorPoint;
                _anchorPoint = currentPoint;
            }
        }

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Textout.Text = "";
            _isDragging = true;
            Mouse.Capture(this, CaptureMode.SubTree);
            _anchorPoint = PointToScreen(e.GetPosition(this));
        }

        private void Circle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Textout.Text = count.ToString();
            _isDragging = false;
            ReleaseMouseCapture();
        }
    }
}
