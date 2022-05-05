using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Shared;

namespace TicTacToe.Client
{
    internal class SocketHandler
    {
        private readonly Socket _server;
        public Game Game { get; private set; }

        public SocketHandler()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Connect(string host, int port)
        {
            try
            {
                _server.Connect(host, port);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to connect to server. Code {e.ErrorCode}");
                CloseConnection();
                return false;
            }

            return true;
        }

        public void HandleMessage(Message expectedMessage)
        {
            var buffer = new byte[8]; // Should never use more than 8 bytes
            var size = _server.Receive(buffer);
            var message = (Message)buffer[0];
            if (message != expectedMessage)
            {
                Console.WriteLine("message != expectedMessage");
                // TODO: error handling stuff
                return;
            }

            switch (message)
            {
                case Message.Start:
                    var symbol = (Symbol)buffer[1];
                    Game = new Game(symbol);
                    break;
            }
        }

        public void CloseConnection()
        {
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }
    }
}
