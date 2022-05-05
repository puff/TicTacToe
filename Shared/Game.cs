namespace TicTacToe.Shared
{
    public class Game
    {
        private readonly Symbol[] Board;
        public bool IsPlaying { get; private set; }
        public Symbol PlayerSymbol { get; private set; }

        // TODO: Refactor this class to work with multiplayer sockets

        public Game(Symbol playerSymbol = Symbol.Empty)
        {
            Board = new Symbol[9];
            for (int i = 0; i < Board.Length; i++)
                Board[i] = Symbol.Empty;

            IsPlaying = true;
            PlayerSymbol = playerSymbol;
        }

        public void PrintBoard()
        {
            var stringBoard = new string[9];
            for (int i = 0; i < stringBoard.Length; i++)
            {
                stringBoard[i] = Board[i] == Symbol.Empty ? " " : Board[i].ToString();
            }
            Console.WriteLine("-+-+-");
            Console.WriteLine(stringBoard[6] + '|' + stringBoard[7] + '|' + stringBoard[8]);
            Console.WriteLine("-+-+-");
            Console.WriteLine(stringBoard[3] + '|' + stringBoard[4] + '|' + stringBoard[5]);
            Console.WriteLine("-+-+-");
            Console.WriteLine(stringBoard[0] + '|' + stringBoard[1] + '|' + stringBoard[2]);
            Console.WriteLine("-+-+-");
        }

        public Symbol CheckWinner()
        {
            // Exclude 0 so it doesn't use Symbol.Empty
            for (var i = 1; i < 3; i++)
            {
                var symbol = (Symbol)i;
                if (
                    (Board[0] == symbol && Board[1] == symbol && Board[2] == symbol) || // First row
                    (Board[3] == symbol && Board[4] == symbol && Board[5] == symbol) || // Second row
                    (Board[6] == symbol && Board[7] == symbol && Board[8] == symbol) || // Third row

                    (Board[6] == symbol && Board[3] == symbol && Board[0] == symbol) || // First column
                    (Board[7] == symbol && Board[4] == symbol && Board[1] == symbol) || // Second column
                    (Board[8] == symbol && Board[5] == symbol && Board[2] == symbol) || // Third column

                    (Board[6] == symbol && Board[4] == symbol && Board[2] == symbol) || // Diagonal1
                    (Board[8] == symbol && Board[4] == symbol && Board[0] == symbol) // Diagonal2
                    )
                {
                    IsPlaying = false;
                    return symbol;
                }
            }
            return Symbol.Empty;
        }

        public bool CheckDraw()
        {
            for (int i = 1; i < Board.Length; i++)
                if (Board[i] == Symbol.Empty)
                    return false;
            return true;
        }

        public bool MakeMove(int move, Symbol symbol)
        {
            if (!(move >= 1 && move <= 9) || Board[move] != Symbol.Empty)
                return false;

            Board[move] = symbol;

            return true;
        }

        // TODO: move this to client only class instead of shared library
        public void StartGame()
        {
            int move;
            while (IsPlaying)
            {
                Console.Clear();
                PrintBoard();

                for (; ; )
                {
                    Console.WriteLine($"You are playing as {PlayerSymbol}.");
                    Console.Write("Please insert a move (1-9): ");
                    if (!int.TryParse(Console.ReadLine(), out move) || !MakeMove(move, PlayerSymbol))
                    {
                        Console.WriteLine("Invalid move.");
                        Thread.Sleep(1200); // 1.2 seconds
                        continue;
                    }

                    break;
                }

                //Board[move] = currentPlayingSymbol;

                //if (CheckWinner(currentPlayingSymbol))
                //{
                //    Console.WriteLine("* * * * * * * * * *");
                //    Console.WriteLine($"'{currentPlayingSymbol}' wins!");
                //    Console.WriteLine("* * * * * * * * * *");
                //    Console.WriteLine("Final board:");
                //    PrintBoard();
                //    Console.WriteLine();
                //    IsPlaying = false;
                //    continue;
                //}
                //else if (CheckDraw())
                //{
                //    Console.WriteLine("* * * * * * * * * *");
                //    Console.WriteLine("It's a draw!");
                //    Console.WriteLine("* * * * * * * * * *");
                //    Console.WriteLine("Final board:");
                //    PrintBoard();
                //    Console.WriteLine();
                //    IsPlaying = false;
                //    continue;
                //}

                //PrintBoard();
                //currentPlayingSymbol = currentPlayingSymbol == Symbol.X ? Symbol.O : Symbol.X; 
            }
        }
    }
}