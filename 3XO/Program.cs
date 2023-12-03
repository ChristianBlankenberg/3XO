
namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GameLogic;
    using global::GameLogic;
    using NeuronalesNetz;
    using NeuronalNet;
    using Newtonsoft.Json;

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
            Console.WriteLine(" 2 - Create Neuronal Net Training Data and Save to File");
            Console.WriteLine(" 3 - load Neuronal Net Training Data from File and train Neuronal Net");
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
                    this.SaveNNTrainingData(
                        this.CreateNNTrainingData(Player.Player),
                        @"C:\temp\nnTrainingData.xml");
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    var trainingData = this.LoadTrainingData(@"C:\temp\nnTrainingData.xml");
                    NeuronalNet neuronalNet = new NeuronalNet(new int[]{ 9, 18, 18, 9 });

                    List<ITrainingPattern> trainData = trainingData.TrainingSet.Cast<ITrainingPattern>().ToList();

                    const double breakCondition = 1e-3;

                    double overAllError = double.PositiveInfinity;
                    for (int i = 0; i < 100000; i++)
                    {
                        Console.WriteLine(neuronalNet.Train(
                            numberOfSteps: 10,
                            learningRate: 0.1,
                            tolerance: breakCondition,
                            trainingPatterns: trainData));

                        Console.WriteLine($"Error (Sum) : {neuronalNet.OverAllError(trainData)}");
                        Console.WriteLine($"Error (Sum) real : {overAllError = neuronalNet.OverAllErrorRealValues(trainData)}");
                    }

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

        private TrainingData LoadTrainingData(string fileName)
        {
            string JSONString = File.ReadAllText(fileName);
            var trainingData = JsonConvert.DeserializeObject<TrainingData>(JSONString);
            return trainingData;
        }

        private void SaveNNTrainingData(TrainingData trainingData, string fileName)
        {
            var trainingDataJSON = JsonConvert.SerializeObject(trainingData);
            File.WriteAllText(fileName, trainingDataJSON);
        }

        private TrainingData CreateNNTrainingData(Player firstPlayer)
        {
            var threeXOBoard = ThreeXOBoard.Empty(firstPlayer);

            double[] GetInputVector(string boardString)
            {
                threeXOBoard.BoardFieldsString = boardString;
                return threeXOBoard.AllFields().Select(p => p.AsInt() - 1.0).ToArray();
            }

            double[] GetOutputVector(int fieldNr)
            {
                var outputVector = Enumerable.Repeat(0.0, 9).ToArray();
                outputVector[fieldNr] = 1.0;
                return outputVector;
            }

            Dictionary<string, int> neuronalNetTrainingData = new Dictionary<string, int>();

            BoardIterator boardIterator = new BoardIterator();
            AlphaBetaPruneClass alphaBetaPruneClass = new AlphaBetaPruneClass(Player.Player, Player.Computer);

            boardIterator.Iterate(ThreeXOBoard.Empty(firstPlayer), 0,
                terminal: (boardIteration, depth) =>
                {
                },
                iteration: (boardIteration, depth) =>
                {
                    if (!neuronalNetTrainingData.ContainsKey(boardIteration.ToString()))
                    {
                        neuronalNetTrainingData.Add(boardIteration.ToString(), GameLogic.GameLogic.GetMaxValueFieldIdx(boardIteration as IBoard, alphaBetaPruneClass));
                    }
                    else
                    {
                        // training data already in dictionary
                    }
                });

            return new TrainingData
            {
                TrainingSet = neuronalNetTrainingData.Select(it => new TrainingsPattern(GetInputVector(it.Key), GetOutputVector(it.Value))).ToList()
            };
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
