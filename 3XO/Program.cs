using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic;
using TicTacToe.GameLogic;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            RunGame();
            //QLearn(new Board(" ; ; ; ;X;X; ; ;O"), Player.Player);
            //QLearn(Board.Empty(), Player.Computer);

        }

        private static void QLearn(Board board, Player player)
        {
            QLearnLogic qLearnLogic = new QLearnLogic();
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
