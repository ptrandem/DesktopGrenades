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

            socket = new Client("http://localhost:8008/"); // url to nodejs 
            //socket.Opened += socket_Opened;
            //socket.Message += SocketMessage;
            //socket.SocketConnectionClosed += SocketConnectionClosed;
            //socket.Error += SocketError;

            // register for 'connect' event with io server
            socket.On("connect", (fn) =>
            {
                Console.WriteLine("\r\nConnected event...\r\n");
            });

            // register for 'update' events - message is a json 'Part' object
            socket.On("news", (data) =>
            {
                Console.WriteLine("  raw message:      {0}", data.RawMessage);
                Console.WriteLine("  string message:   {0}", data.MessageText);
            });

            // make the socket.io connection
            socket.Connect();
            socket.Emit("my other event", new {data = "some test payload"});
            var running = true;

            while (running)
            {
                var input = Console.ReadLine();
                if (input == "c")
                {
                    socket.Emit("checkin", new {checkin = ".net client"});
                }
                else if (input == "x")
                {
                    running = false;
                }
            }
            socket.Dispose();
        }

        static void SocketError(object sender, ErrorEventArgs e)
        {
            
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
