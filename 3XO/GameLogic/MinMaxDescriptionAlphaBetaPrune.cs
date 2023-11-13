using System;
using System.Collections.Generic;
using TicTacToe.GameLogic;

namespace GameLogic
{
    public class MinMaxDescriptionAlphaBetaPrune : MinMaxDescription
    {
        public MinMaxDescriptionAlphaBetaPrune(
            Player player,
            int startValue,
            Func<double, double, double> minMaxFunc,
            Func<List<(int idx, double value)>, double> minMaxListFunc,
            bool alpha,
            bool beta) : base(player, startValue, minMaxFunc, minMaxListFunc) 
        {
            this.Alpha = alpha;
            this.Beta = beta;
        }

        public bool Alpha { get; }
        public bool Beta { get; }
    }
}
