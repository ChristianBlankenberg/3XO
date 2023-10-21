using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.GameLogic;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleGame consoleGame = new ConsoleGame(new Game(Board.Empty()));

            consoleGame.Test();

            consoleGame.Run();
        }
    }
}
