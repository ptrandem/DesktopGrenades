using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using System.Linq;
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
            _timer.Interval = 10;
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
            _circleBody.ApplyTorque(500f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(_area.Width*4),
                                                      ConvertUnits.ToSimUnits(1f), 1f,
                                                      ConvertUnits.ToSimUnits(new Vector2(_area.Width/2, _area.Height)));
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.8f;
            _groundBody.Friction = 0.5f;

            // Create east wall
            _groundBody2 = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(1f),
                                                       ConvertUnits.ToSimUnits(_area.Height), 1f,
                                                       ConvertUnits.ToSimUnits(new Vector2(_area.Width * 2 - 50,
                                                                                           _area.Height)));
            _groundBody2.IsStatic = true;
            _groundBody2.Restitution = 0.8f;
            _groundBody2.Friction = 0.5f;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _world.Step(.025f);
            Top = ConvertUnits.ToDisplayUnits(_circleBody.Position.Y) - 50;
            Left = ConvertUnits.ToDisplayUnits(_circleBody.Position.X) - 50;
            Rotation.Angle = _circleBody.Rotation * 57.2957795;
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var circlePoint = e.GetPosition(this);
                var screenPoint = PointToScreen(circlePoint);
                var position = GetVector2(screenPoint);
                if (_mouseJoint != null)
                {
                    _mouseJoint.WorldAnchorB = ConvertUnits.ToSimUnits(position);
                }
                //Left = Left + currentPoint.X - _anchorPoint.X;
                //Top = Top + currentPoint.Y - _anchorPoint.Y;
                //_anchorDelta = currentPoint - _anchorPoint;
                //_anchorPoint = currentPoint;
                //_mouseJoint.
            }
        }

        private Vector2 GetVector2(Point point)
        {
            return new Vector2((float)point.X, (float) point.Y);
        }

        private FixedMouseJoint _mouseJoint;
        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            Mouse.Capture(this, CaptureMode.SubTree);
            var circlePoint = e.GetPosition(this);
            var screenPoint = PointToScreen(circlePoint);
            var position = GetVector2(screenPoint);
            Textout.Text = "down";

            var fixture = _world.TestPoint(ConvertUnits.ToSimUnits(position));

            //var cpc = ConvertUnits.ToDisplayUnits(_circleBody.Position);
            //Textout.Text = string.Format("cp:{0}\nsp:{1}\np:{2}\ncpc:{3}",
            //                             circlePoint.ToString(),
            //                             screenPoint.ToString(),
            //                             position.ToString(),
            //                             cpc.ToString());

            if (fixture != null)
            {
                Body body = fixture.Body;
                _mouseJoint = new FixedMouseJoint(body, ConvertUnits.ToSimUnits(position)) {MaxForce = 10000.0f*body.Mass};
                _world.AddJoint(_mouseJoint);
                body.Awake = true;
            }
        }

        private void Circle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Textout.Text = "up";
            //if (_mouseJoint != null)
            //{
            //    _world.RemoveJoint(_mouseJoint);
            //}
            //_world.RemoveJoint();
            _world.JointList.Where(j => j.JointType == JointType.FixedMouse).ToList().ForEach(_world.RemoveJoint);

            //_mouseJoint = null;
            //Textout.Text = count.ToString();
            _isDragging = false;
            ReleaseMouseCapture();
        }
    }
}
