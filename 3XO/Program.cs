
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
            Console.WriteLine(" 2 - Create Neuronal Net Training Data");
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
                    this.CreateNNTrainingData(Player.Player);
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

        private void CreateNNTrainingData(Player firstPlayer)
        {
            BoardIterator boardIterator = new BoardIterator();
            AlphaBetaPruneClass alphaBetaPruneClass = new AlphaBetaPruneClass(Player.Player, Player.Computer);

            boardIterator.Iterate(ThreeXOBoard.Empty(firstPlayer), 0,
                (boardIteration, depth) =>
                {             
                    Console.WriteLine($"{boardIteration.ToString()} : {GameLogic.GameLogic.GetMaxValueFieldIdx(boardIteration as IBoard, alphaBetaPruneClass)}");
                });                
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
