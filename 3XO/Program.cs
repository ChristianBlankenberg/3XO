using System;
using GameLogic;
using TicTacToe.GameLogic;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            RunGame();
            //QLearn(new Board("X; ;O; ;O; ;X; ;X"), Player.Player);
            //QLearn(Board.Empty(), Player.Computer);
            Console.ReadLine();
        }

        private static void QLearn(Board board, Player player)
        {
            QLearnLogic qLearnLogic = new QLearnLogic((s) => Console.WriteLine(s));
            qLearnLogic.QLearn(board, player);
        }

        private static void RunGame()
        {
            ConsoleGame consoleGame = new ConsoleGame(new Game(Board.Empty()));
            consoleGame.Test();
            consoleGame.Run();
        }
    }
}
