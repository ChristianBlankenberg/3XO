using System;
using System.Collections.Generic;
using TicTacToe.GameLogic;

namespace GameLogic
{
    public class MinMaxDescription
    {
        public MinMaxDescription(
            Player player, 
            int startValue,
            Func<double, double, double> minMaxFunc,
            Func<List<(int idx, double value)>, double> minMaxListFunc)
        {
            this.Player = player;
            this.StartValue = startValue;
            this.MinMaxFunc = minMaxFunc;
            this.MinMaxListFunc = minMaxListFunc;
        }

        public Player Player { get; }
        public int StartValue { get; }
        public Func<double, double, double> MinMaxFunc { get; }
        public Func<List<(int idx, double value)>, double> MinMaxListFunc { get; }
    }
}
