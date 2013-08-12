using System;
using System.Net.Sockets;
using SocketIOClient;
using WebSocket4Net;

namespace WebSocketClientTest
{
    class Program
    {

        static void Main(string[] args)
        {
            Execute();

        }

        private static void Execute()
        {
            Client socket;
            Console.WriteLine("Starting TestSocketIOClient Example...");

            socket = new Client("http://localhost:8008/"); // url to nodejs server
            //socket.Opened += socket_Opened;
            //socket.Message += SocketMessage;
            //socket.SocketConnectionClosed += SocketConnectionClosed;
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\r\nPress 'c' to execute checkin event \r\n\t(view socket.io server console window to verify).\r\nPress 'x' to exit.");
            Console.ResetColor();

            // make the socket.io connection
            socket.Connect();

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
    }
}
