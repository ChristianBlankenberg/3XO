
namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using GameLogic;
    using global::GameLogic;
    
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();
        }

        private Program()
        {
            this.CreateMenu(new List<(string, Action)>
            {
                ("Run Game (Player begins)", () => { this.RunGame(Player.Player); }),
                ("Run Game (Computer begins)", () => { this.RunGame(Player.Computer); }),
                ("Test", () => { this.Test(); }),
                ("Debug", () => { this.Debug(); }),
            }) ;
        }

        private void CreateMenu(List<(string, Action)> menuList)
        {
            //this.Men
        }

        private void Start()
        {
            Console.WriteLine(" --- 3XO --- ");
            Console.WriteLine("");
            Console.WriteLine(" 1 - Run Game ");
            Console.WriteLine(" 4 - Test ");
            Console.WriteLine(" 5 - Debug ");

            Console.WriteLine("");

            var key = Console.ReadKey();

            switch(key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    this.RunGame(Player.Player);
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    this.Test();
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    this.Debug();
                    break;
            }
        }

        private void Debug()
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty(Player.Computer), Player.Computer));
            consoleGame.Run();

            Console.ReadLine();
        }

        private void RunGame(Player startPlayer)
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty(startPlayer), startPlayer));
            consoleGame.Run();

            Console.ReadLine();
        }

        private void Test()
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty(Player.Player), Player.Player));
            consoleGame.Test();

            Console.ReadLine();
        }
    }
}
