using System.Net.Sockets;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    internal class Player
    {
        public Socket Client { get; private set; }
        public Symbol PlayerSymbol { get; private set; }

        public Player(Socket client, Symbol playerSymbol)
        {
            Client = client;
            PlayerSymbol = playerSymbol;
        }
    }
}
