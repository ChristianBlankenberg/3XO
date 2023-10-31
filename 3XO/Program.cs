using System;
using GameLogic;
using TicTacToe.GameLogic;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunGame();
            //QLearn(new Board<PlayerComputer>(" ; ; ; ;X;X; ; ;O"), Player.Player);
            QLearn(Board<PlayerComputer>.Empty(), Player.Computer);

            Console.ReadLine();
        }

        private static void QLearn(Board<PlayerComputer> board, Player player)
        {
            QLearnLogic qLearnLogic = new QLearnLogic((s) => Console.WriteLine(s));
            qLearnLogic.QLearn(board, player);
        }

        private static void RunGame()
        {
            ConsoleGame consoleGame = new ConsoleGame(new Game(Board<PlayerComputer>.Empty()));
            consoleGame.Test();
            consoleGame.Run();
        }
    }
}
