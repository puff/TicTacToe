using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Shared;

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

        #region Setup
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


            while (_players[1] == null)
            {
                try
                {
                    Console.WriteLine("Attempting to listen for connections...");
                    _server.Listen(1);

                    var client = _server.Accept();
                    var firstPlayer = true;
                    if (_players.FirstOrDefault() != null)
                    {
                        firstPlayer = false;
                        _players[1] = new Player(client, _players[0].PlayerSymbol == Shared.Symbol.X ? Shared.Symbol.O : Shared.Symbol.X);
                    }
                    else
                        _players[0] = new Player(client, (Shared.Symbol)new Random().Next(1, 2));

                    Console.WriteLine($"Client #{(firstPlayer ? 1 : 2)} - {client.LocalEndPoint} connected. Symbol: {_players[firstPlayer ? 0 : 1].PlayerSymbol}");
                    
                    if (!firstPlayer)
                        return true;
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"Failed to listen on server. Code {e.ErrorCode}");
                    CloseConnection();
                    return false;
                }
            }

            return false;
        }
        #endregion

        #region Game Methods
        public void NotifyPlayers(Shared.Message message, bool fromClient = false, Player? clientPlayer = null)
        {
            switch (message)
            {
                case Shared.Message.Start:
                    foreach (var player in _players)
                        // TODO: notify players to begin match
                        break;
                    break;
                case Shared.Message.Move:

                    break;
                case Shared.Message.End:

                    break;
                case Shared.Message.Rematch:

                    break;
            }
        }


        #endregion

        public void CloseConnection()
        {
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }
    }
}
