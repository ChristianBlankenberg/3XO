using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuronalNet;

namespace TicTacToe.GameLogic
{
    internal class GameNeuronalNet
    {
        INeuronalesNetz neuronalesNetz;

        internal GameNeuronalNet()
        {
            this.neuronalesNetz = new NeuronalesNetz();
            this.neuronalesNetz.GenerateNeuronalNet(new int[] { 9, 12, 9 });
        }

        internal Coordinates GetOutput(Board board)
        {
            double[] output = this.neuronalesNetz.Calculate(this.GetInputLayer(board));

            var index = output.ToList().FindIndex(x => x == output.Max());

            return new Coordinates(index);
        }

        internal void Init()
        {
            List<ITrainingsMuster> trainingsMuster = new List<ITrainingsMuster>();

            for (int i = 0; i < 9; i++)
            {
                trainingsMuster.Add(new Trainingsmuster(this.Get3XOVector(i), this.Get3XOVector(8 - i)));
            }

            this.neuronalesNetz.Train(1000000, 0.3, 0.001, trainingsMuster);
        }

        internal void Train()
        {
            this.SetOnBoard(Board.Empty(), Player.Player);
        }

        private void SetOnBoard(Board board, Player player)
        {
            if (board.Full() || board.Winner() != Player.None)
            {
                var lines = board.Print();

                foreach (var line in lines)
                {
                    File.AppendAllText("3XO.txt", $"{line}{Environment.NewLine}");
                }

                File.AppendAllText("3XO.txt", $"{Environment.NewLine}");
            }
            else
            {
                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    Board newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr) == Player.None)
                    {
                        newBoard.Set(fieldNr, player);
                        this.SetOnBoard(newBoard, player == Player.Player ? Player.Computer : Player.Player);
                    }   
                }
            }
        }

        internal List<string> Test()
        {
            List<ITrainingsMuster> trainingsMuster = new List<ITrainingsMuster>();

            for (int i = 0; i < 9; i++)
            {
                trainingsMuster.Add(new Trainingsmuster(this.Get3XOVector(i), this.Get3XOVector(8 - i)));
            }

            List<string> result = new List<string>();
            foreach (Trainingsmuster tm in trainingsMuster)
            {
                string outStr = string.Join(", ", tm.EingabeVektor);
                double[] ausgabe = this.neuronalesNetz.Calculate(tm.EingabeVektor);
                outStr += " : ";
                outStr += string.Join(", ", ausgabe);
                result.Add(outStr);
            }

            return result;
        }

        private double[] GetInputLayer(Board board) => board.Fields().Select(b => b.AsDouble()).ToArray();

        private double[] Get3XOVector(int idx)
        {
            double[] result = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            result[idx] = 1;

            return result;
        }
    }
}
