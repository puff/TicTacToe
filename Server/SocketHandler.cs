using System.Net;
using System.Net.Sockets;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    internal class SocketHandler
    {
        private readonly Socket _server;
        private readonly Player[] _players;
        private readonly Game _game;

        public SocketHandler(Game game)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _players = new Player[2];
            _game = game;
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
        public void HandleMessage(Message expectedMessage, Symbol expectedSymbol)
        {
            // TODO: add verification checks to be sure message came from correct client (ip endpoint)

            var buffer = new byte[8]; // Should never use more than 8 bytes
            var size =_server.Receive(buffer);
            var message = (Message)buffer[0];
            if (message != expectedMessage)
            {
                Console.WriteLine("message != expectedMessage");
                // TODO: error handling stuff
                return;
            }

            var symbol = (Symbol)buffer[1];
            if (symbol != expectedSymbol)
            {
                Console.WriteLine("symbol != expectedSymbol");
                // TODO: error handling stuff
                return;
            }

            // TODO: rematch stuff
            switch (message)
            {
                case Message.Move:
                    var move = buffer[2]; 
                    if (!_game.MakeMove(move, symbol))
                    {
                        Console.WriteLine("Invalid move from " + symbol.ToString());
                        // TODO: error handling stuff
                    }

                    var winner = _game.CheckWinner();
                    if (winner != Symbol.Empty)
                    {
                        // Notify other player to update their board and end the game
                        NotifyPlayers(Message.End, null, new byte[3] 
                        {
                            move,
                            0, // not a draw
                            (byte)winner
                        });
                    }
                    else if (_game.CheckDraw())
                    {
                        // Notify other player to update their board and end the game with a draw
                        NotifyPlayers(Message.End, null, new byte[2]
                        {
                            move,
                            1 // draw
                        });
                    }
                    else
                    {
                        // Notify other player to update their board and send a move back to server
                        NotifyPlayers(Message.Move, _players.Where(_player => _player.PlayerSymbol != symbol).FirstOrDefault(), new byte[1] { move });
                    }

                    
                    break;
            }
        }

        public void NotifyPlayers(Message message, Player? toPlayer = null, byte[] messageBuffer = null)
        {
            switch (message)
            {
                case Message.Start:
                    foreach (var player in _players)
                        player.Client.Send(new byte[2]
                        {
                            (byte)Message.Start,
                            (byte)player.PlayerSymbol
                        });
                    break;
                case Message.Move:
                    toPlayer?.Client.Send(new byte[3]
                    {
                        (byte)Message.Move,
                        (byte)(toPlayer.PlayerSymbol == Symbol.X ? Symbol.O : Symbol.X),
                        messageBuffer[0],
                    });
                    break;
                case Message.End:
                    foreach (var player in _players)
                        player.Client.Send(new byte[4]
                        {
                            (byte)Message.End,
                            messageBuffer[0], // move
                            messageBuffer[1], // draw
                            messageBuffer[2] // winner (if it's not a draw)
                        });
                    break;
                //case Message.Rematch:

                //    break;
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
