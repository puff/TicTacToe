using System;
using System.Text.RegularExpressions;
using TicTacToe.Shared;

namespace TicTacToe.Client // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        const string ipRegexValidator = "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        static void Main(string[] args)
        {
            string ip;
            int port;
            for (; ;)
            {
                Console.Clear();
                Console.Write("Enter ip of server: ");
                ip = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(ip) || !Regex.IsMatch(ip, ipRegexValidator))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid ip address.");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    continue;
                }
                break;
            }

            for (; ; )
            {
                Console.Clear();
                Console.Write("Enter port of server: ");
                var tPort = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tPort) || tPort.Length <= 0 || tPort.Length > 5 || !int.TryParse(tPort, out port))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid port.");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    continue;
                }
                break;
            }

            // TODO: socket stuff
            var socketHandler = new SocketHandler();

            if (socketHandler.Connect(ip, port))
            {
                Console.WriteLine("Connected! Waiting for start message...");
                socketHandler.HandleMessage(Message.Start);

                var game = socketHandler.Game;
                if (game == null)
                {
                    Console.WriteLine("Failed to get game...");
                    socketHandler.CloseConnection();
                    Thread.Sleep(1500);
                    Environment.Exit(0);
                }

                int move;
                while (game.IsPlaying)
                {
                    Console.Clear();
                    game.PrintBoard();

                    for (; ; )
                    {
                        Console.WriteLine($"You are playing as {game.PlayerSymbol}.");
                        Console.Write("Please insert a move (1-9): ");
                        if (!int.TryParse(Console.ReadLine(), out move) || !game.MakeMove(move, game.PlayerSymbol))
                        {
                            Console.WriteLine("Invalid move.");
                            Thread.Sleep(1200); // 1.2 seconds
                            continue;
                        }

                        break;
                    }
                }
            }
        }
    }
}