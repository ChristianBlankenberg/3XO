using System;
using GameLogic;
using TicTacToe.GameLogic;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();
        }

        private void Start()
        {
            Console.WriteLine(" --- 3XO --- ");
            Console.WriteLine("");
            Console.WriteLine(" 1 - Run Game ");
            Console.WriteLine(" 2 - Q Learn  ");

            var key = Console.ReadKey();

            switch(key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    this.RunGame();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    this.QLearn(ThreeXOBoard.Empty(), Player.Player);
                    break;
            }
        }


        private void QLearn(ThreeXOBoard board, Player player)
        {
            QLearnLogic<ThreeXOBoard> qLearnLogic = new QLearnLogic<ThreeXOBoard>((s) => Console.WriteLine(s));
            qLearnLogic.QLearn(board, player);

            Console.ReadLine();
        }

        private void RunGame()
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty()));
            consoleGame.Test();
            consoleGame.Run(startPlayer: Player.Computer);

            Console.ReadLine();
        }
    }
}
