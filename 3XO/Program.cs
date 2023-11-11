
namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using GameLogic;
    using global::GameLogic;
    using TicTacToe.GameLogic;

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
                ("Q-Learn", () => { this.QLearn(ThreeXOBoard.Empty(), Player.Player); }),
                ("Q-Learn (Short)", () => { this.QLearn(new ThreeXOBoard("X;O;X;O;X;O; ; ; "), Player.Player); }),
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
            Console.WriteLine(" 2 - Q Learn  ");
            Console.WriteLine(" 3 - Q Learn (Short) ");
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
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    this.QLearn(ThreeXOBoard.Empty(), Player.Player);
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    this.QLearn(new ThreeXOBoard("X;O;X;O;X;O; ; ; "), Player.Player);
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


        private void QLearn(ThreeXOBoard board, Player player)
        {
            QLearnLogic<ThreeXOBoard> qLearnLogic = new QLearnLogic<ThreeXOBoard>((s) => Console.WriteLine(s));
            qLearnLogic.QLearn(board, player);

            Console.ReadLine();
        }

        private void Debug()
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty()));
            consoleGame.Run(new ThreeXOBoard("X;X;O;X;O; ; ; ; "), startPlayer: Player.Computer);

            Console.ReadLine();
        }

        private void RunGame(Player startPlayer)
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty()));
            consoleGame.Run(ThreeXOBoard.Empty(), startPlayer: startPlayer);

            Console.ReadLine();
        }

        private void Test()
        {
            ConsoleGame<ThreeXOBoard> consoleGame = new ConsoleGame<ThreeXOBoard>(new Game(ThreeXOBoard.Empty()));
            consoleGame.Test();

            Console.ReadLine();
        }
    }
}
