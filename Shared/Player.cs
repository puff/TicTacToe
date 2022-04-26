using System.Net.Sockets;

namespace TicTacToe.Shared
{
    public class Player
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
