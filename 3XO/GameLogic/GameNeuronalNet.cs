using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuronalNet;
using NeuronalNet.Data;

namespace TicTacToe.GameLogic
{
    internal class GameNeuronalNet
    {
        List<List<double>> values;
        List<List<double[]>> weights;

        internal GameNeuronalNet()
        {
            this.weights = new List<List<double[]>>();            
            this.weights.Add(Enumerable.Repeat(Enumerable.Repeat(0.0, 9).ToArray(), 400).ToList());
            this.weights.Add(Enumerable.Repeat(Enumerable.Repeat(0.0, 400).ToArray(), 9).ToList());

            this.values = new List<List<double>>();
            this.values.Add(Enumerable.Repeat(0.0, 9).ToList());
            this.values.Add(Enumerable.Repeat(0.0, 400).ToList());
            this.values.Add(Enumerable.Repeat(0.0, 9).ToList());

            this.LoadAndSetWeights(0, "wih.csv");
            this.LoadAndSetWeights(1, "who.csv");
        }

        internal void LoadAndSetWeights(int layerIdx, string csvFilename)
        {
            var fileLines = File.ReadAllLines(csvFilename);

            if (fileLines.Count() == this.weights[layerIdx].Count)
            {
                for (int lineNr = 0; lineNr < fileLines.Count(); lineNr++)
                {
                    var fileLine = fileLines[lineNr];
                    double[] doubleVals = fileLine.Split(new string[] { "," }, StringSplitOptions.None).Select(s => Convert.ToDouble(s.Replace(".", ","))).ToArray();

                    this.weights[layerIdx][lineNr] = doubleVals.ToArray();
                }
            }
            else
            {
                throw new ArithmeticException("fileLines.Count() == layer.Count");
            }
        }

        internal Coordinates GetOutput(IBoard board)
        {
            this.values[0] = this.GetInputLayer(board).ToList();

            for (int layerIdx = 1; layerIdx < 3; layerIdx++)
            {
                for (int valueIdx = 0; valueIdx < this.values[layerIdx].Count; valueIdx++)
                {
                    this.values[layerIdx][valueIdx] = 0;
                    for (int weightIdx = 0; weightIdx < this.weights[layerIdx-1][valueIdx].Length; weightIdx++)
                    {
                        this.values[layerIdx][valueIdx] += this.weights[layerIdx-1][valueIdx][weightIdx] * this.values[layerIdx-1][weightIdx];
                    }

                    this.values[layerIdx][valueIdx] = this.Sigmoid(this.values[layerIdx][valueIdx]);
                }
            }

            var index = this.values[2].ToList().FindIndex(x => x == this.values[2].Max());
            return new Coordinates(index);
        }

        public double Sigmoid(double value)
        {
            return (1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        private double[] GetInputLayer(IBoard board) => board.AllFields().Select(b => b.AsInt() - 1.0).ToArray();
    }
}
