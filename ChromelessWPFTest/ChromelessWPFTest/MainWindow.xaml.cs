using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
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

        

        //TODO: CLEAN UP THIS HORRIBLE HORRIBLE MESSY CODE MASHUP

        private LowLevelMouseProc _proc;
        private IntPtr _hookId = IntPtr.Zero;

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
            _proc = HookCallback;

            _hookId = SetHook(_proc);
            //Application.Run();


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
            var circlePos = e.GetPosition(this);
            var screenPoint = PointToScreen(circlePos);
            OnMouseMove(screenPoint);
        }

        private void OnMouseMove(Point position)
        {
            if (_isDragging)
            {
                var circlePosition = GetVector2(position);

                Debug.Print("dragging:" + position + "\t" + circlePosition);
                if (_mouseJoint != null)
                {
                    _mouseJoint.WorldAnchorB = ConvertUnits.ToSimUnits(circlePosition);
                }
            }
        }

        private Vector2 GetVector2(Point point)
        {
            return new Vector2((float)point.X, (float) point.Y);
        }

        private FixedMouseJoint _mouseJoint;
        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.Print("Circle_MouseLeftButtonDown");
            if (_isDragging) return;
            Debug.Print("Circle_MouseLeftButtonDown - drag start");
            //Mouse.Capture(this, CaptureMode.SubTree);
            //this.CaptureMouse();
            _isDragging = true;
            var circlePoint = e.GetPosition(this);
            var screenPoint = PointToScreen(circlePoint);
            var screenPosition = GetVector2(screenPoint);
            var worldPosition = ConvertUnits.ToSimUnits(screenPosition);

            Textout.Text = "down";

            var fixture = _world.TestPoint(worldPosition);

            if (fixture != null)
            {
                Body body = fixture.Body;
                _mouseJoint = new FixedMouseJoint(body, worldPosition) { MaxForce = 1000000.0f * body.Mass };
                _world.AddJoint(_mouseJoint);
                body.Awake = true;
            }
        }

        private void Circle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.Print("Circle_OnMouseLeftButtonUp");
            if (!_isDragging) return;
            Debug.Print("Circle_OnMouseLeftButtonUp - drag stopping");
            _isDragging = false;
            Textout.Text = "up";
            _world.JointList.Where(j => j.JointType == JointType.FixedMouse).ToList().ForEach(_world.RemoveJoint);
            ReleaseMouseCapture();
        }

        //private void circle_LostMouseCapture(object sender, MouseEventArgs e)
        //{
        //    Debug.Print("circle_LostMouseCapture");

        //    if (!_isDragging) return;

        //    Debug.Print("circle_LostMouseCapture - drag stopping");

        //    _isDragging = false;
            
        //    Textout.Text = "up";
        //    _world.JointList.Where(j => j.JointType == JointType.FixedMouse).ToList().ForEach(_world.RemoveJoint);
        //}

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 &&
                MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
            {
                Circle_OnMouseLeftButtonUp(this, null);
            }
            
            var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            OnMouseMove(new Point(hookStruct.pt.x, hookStruct.pt.y));
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            UnhookWindowsHookEx(_hookId);            
        }
    }
}
