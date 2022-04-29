﻿using System;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    internal class Program
    {

        private const int Port = 6236; // Random unassigned port

        static void Main(string[] args)
        {
            Console.ResetColor();

            var game = new Game();
            var socketHandler = new SocketHandler(game);
            var localIP = GetLocalIP();
            if (!socketHandler.TryBindSocket(localIP, Port))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to bind server.");
                Console.ResetColor();
                socketHandler.CloseConnection();
                Console.ReadLine();
                Environment.Exit(-1);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Server bound to {localIP}:{Port}");
            Console.ResetColor();

            if (!socketHandler.TryListen())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to listen on server.");
                Console.ResetColor();
                socketHandler.CloseConnection();
                Console.ReadLine();
                Environment.Exit(-1);
            }

            // Both clients have connected, start game loop now.
            while (game.IsPlaying)
            {

                break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press enter to exit program...");

            Console.ReadLine();
            Console.WriteLine("Exiting...");
            Console.ResetColor();

            socketHandler.CloseConnection();
            Thread.Sleep(1500); // 1.5 seconds
        }

        // https://stackoverflow.com/a/27376368
        static string GetLocalIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }
    }
}