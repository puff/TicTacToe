using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Server
{
    internal class SocketHandler
    {
        private readonly Socket _server;
        private readonly Player[] _players;

        public SocketHandler()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _players = new Player[2];
        }

        public bool TryBindSocket(string localIP, int port)
        {
            try
            {
                _server.Bind(new IPEndPoint(IPAddress.Parse(localIP), port));
                return true;
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to setup socket. Code {e.ErrorCode}");
                CloseConnection();
                return false;
            }
        }

        public bool TryListen()
        {
            if (!_server.IsBound)
            {
                Console.WriteLine("Server is not bound!");
                return false;
            }
            
            try
            {
                Console.WriteLine("Attempting to listen for connections...");
                _server.Listen(1);

                var client = _server.Accept();
                if (_players.FirstOrDefault() != null)
                    _players[1] = new Player(client, Shared.Symbol.O);
                else
                    _players[0] = new Player(client, Shared.Symbol.X);

                Console.WriteLine($"Accepted connection from {client.LocalEndPoint}");
                return true;
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to listen on server. Code {e.ErrorCode}");
                CloseConnection();
                return false;
            }
        }

        public void CloseConnection()
        {
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }
    }
}
