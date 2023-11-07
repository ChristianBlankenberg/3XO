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
            //QLearn(new ThreeXOBoard("X; ;O; ;O; ;X; ;X"), Player.Player);
            //QLearn(ThreeXOBoard.Empty(), Player.Computer);
            Console.ReadLine();
        }

        private static void QLearn(ThreeXOBoard board, Player player)
        {
            QLearnLogic<ThreeXOBoard> qLearnLogic = new QLearnLogic<ThreeXOBoard>((s) => Console.WriteLine(s));
            qLearnLogic.QLearn(board, player);
        }

        private static void RunGame()
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty()));
            consoleGame.Test();
            consoleGame.Run();
        }
    }
}
