using System;
using System.Drawing;
using System.Windows.Forms;
using SocketIOClient;

using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Windows.Forms;

namespace WebSocketClientTest
{
    class Program
    {
        public static Client socket;
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        static void Main(string[] args)
        {

            Console.WriteLine("Starting TestSocketIOClient Example...");
            socket = new Client("http://localhost:8008/"); // url to nodejs server
            socket.Error += SocketError;

            // register for 'connect' event with io server
            socket.On("connect", (fn) =>
            {
                Console.WriteLine("\r\nConnected event...\r\n");
            });

            socket.On("news", (data) =>
            {
                Console.WriteLine("  raw message:      {0}", data.RawMessage);
                Console.WriteLine("  string message:   {0}", data.MessageText);
            });

            socket.On("acceptPosition", (data) =>
                {
                    var x = ((int) data.Json.Args[0].x.Value);
                    var y = ((int) data.Json.Args[0].y.Value);
                    var point = new System.Drawing.Point(x,y);
                    //Console.WriteLine("external: " + point);
                    SetCursorPos(point.X, point.Y);
                });


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\r\nPress 'c' to execute checkin event \r\n\t(view socket.io server console window to verify).\r\nPress 'x' to exit.");
            Console.ResetColor();

            // make the socket.io connection
            socket.Connect();


            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);

            socket.Dispose();
            //Execute();

            
        }

        private static void Execute()
        {

            // example test event
            socket.Emit("my other event", new { data = "some test payload" });
            var running = true;


            while (running)
            {
                var input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.C)
                {
                    if (socket.IsConnected)
                    {
                        socket.Emit("checkin", new {checkin = ".net client"});
                        Console.WriteLine("Checkin sent.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not connected.");
                        Console.ResetColor();
                    }
                    
                }
                else if (input.Key == ConsoleKey.X)
                {
                    running = false;
                }
            }
            socket.Dispose();
        }

        static void SocketError(object sender, ErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Socket error: " + e.Message);
            Console.ResetColor();
        }

        private static void SocketConnectionClosed(object sender, EventArgs e)
        {
        }

        private static void SocketMessage(object sender, MessageEventArgs e)
        {
        }

        static void socket_Opened(object sender, EventArgs e)
        {
        }

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

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            //if (nCode >= 0 &&
            //    MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            //{
                var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                socket.Emit("positionChange", new { x = hookStruct.pt.x, y = hookStruct.pt.y });
                Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
            //}
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
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
    }
}
