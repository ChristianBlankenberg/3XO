
namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using TicTacToe.GameLogic;

    internal class ConsoleGame<BoardType> where BoardType : class, IBoard
    {
        private Game game;

        internal ConsoleGame(Game game)
        {
            this.game = game;
        }

        internal void Run() => this.game.Run(this.GetCoordinatesFromConsole, this.PrintBoard);

        internal void Test() => this.game.Test(this.PrintBoard, () => { Console.ReadKey(); });

        private void PrintBoard(List<string> lines)
        {
            Console.Clear();

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        private Coordinates GetCoordinatesFromConsole()
        {
            bool Is0(ConsoleKeyInfo consoleKeyInfo) => consoleKeyInfo.Key == ConsoleKey.D0 || consoleKeyInfo.Key == ConsoleKey.NumPad0;

            bool Is1(ConsoleKeyInfo consoleKeyInfo) => consoleKeyInfo.Key == ConsoleKey.D1 || consoleKeyInfo.Key == ConsoleKey.NumPad1;

            bool Is2(ConsoleKeyInfo consoleKeyInfo) => consoleKeyInfo.Key == ConsoleKey.D2 || consoleKeyInfo.Key == ConsoleKey.NumPad2;

            int GetNr(ConsoleKeyInfo consoleKeyInfo)
            {
                return
                    Is0(consoleKeyInfo) ? 0 :
                        Is1(consoleKeyInfo) ? 1 :
                            Is2(consoleKeyInfo) ? 2 :
                                -1;
            }

            ConsoleKeyInfo spalte;
            do
            {
                spalte = Console.ReadKey();
            }
            while (!Is0(spalte) && !Is1(spalte) && !Is2(spalte));

            ConsoleKeyInfo reihe;
            do
            {
                reihe = Console.ReadKey();
            }
            while (!Is0(reihe) && !Is1(reihe) && !Is2(reihe));

            return new Coordinates(GetNr(spalte), GetNr(reihe));
        }
    }
}
