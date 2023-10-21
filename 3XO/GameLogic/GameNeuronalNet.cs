using System;
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
            this.neuronalesNetz.GenerateNeuronalNet(new int[] {9, 12, 9});
        }

        internal Coordinates GetOutput(Board board)
        {
            double[] output = this.neuronalesNetz.Calculate(this.GetInputLayer(board));

            var index = output.ToList().FindIndex(x => x == output.Max());

            return new Coordinates(index);
        }

        private double[] GetInputLayer(Board board) => board.Fields().Select(b => b.AsDouble()).ToArray();
    }
}
