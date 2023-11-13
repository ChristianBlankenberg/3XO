
namespace GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TicTacToe.GameLogic;
    
    public class GameLogicAlphaBetaPrune
    {
        private readonly MinMaxDescriptionAlphaBetaPrune minMaxP1;
        private readonly MinMaxDescriptionAlphaBetaPrune minMaxP2;
        private readonly Random random = new Random();
        private double alpha;
        private double beta;

        public GameLogicAlphaBetaPrune(MinMaxDescriptionAlphaBetaPrune minMaxP1, MinMaxDescriptionAlphaBetaPrune minMaxP2)
        {
            this.minMaxP1 = minMaxP1;
            this.minMaxP2 = minMaxP2;
        }

        public int GetFavouriteFieldIdx(IBoard board, Player player)
        {
            MinMaxDescription minMaxDescription = player == minMaxP1.Player ? minMaxP1 : player == minMaxP2.Player ? minMaxP2 : null;

            var emptyFieldIdxs = board.GetEmptyFieldIdxs();
            List<(int idx, double value)> idxAndValue = new List<(int idx, double value)>(); 

            foreach(var emptyFieldIdx in emptyFieldIdxs)
            {
                board.Set(emptyFieldIdx, player);

                this.alpha = int.MinValue;
                this.beta = int.MaxValue;

                idxAndValue.Add((emptyFieldIdx, this.GetValueOfBoardAlphaBetaPrune(board, player, 0)));
                board.Set(emptyFieldIdx, Player.None);
            }

            var minMaxValue = minMaxDescription.MinMaxListFunc(idxAndValue);
            var minMaxValuesAndIdxs = idxAndValue.Where(x => x.value == minMaxValue).ToList();            

            return minMaxValuesAndIdxs[random.Next(0, minMaxValuesAndIdxs.Count)].idx;
        }

        private double GetValueOfBoardAlphaBetaPrune(IBoard board, Player player, int depth)
        {
            if (board.IsTerminal())
            {
                return board.GetValue();
            }
            else
             {
                var opponent = player.Opponent();
                var emptyFieldIdxs = board.GetEmptyFieldIdxs();
                MinMaxDescriptionAlphaBetaPrune minMaxDescriptionAlphaBetaPrune = opponent == minMaxP1.Player ? minMaxP1 : opponent == minMaxP2.Player ? minMaxP2 : null;

                double value = minMaxDescriptionAlphaBetaPrune.StartValue;

                foreach(var emptyFieldIdx in emptyFieldIdxs)
                {
                    board.Set(emptyFieldIdx, opponent);

                    value = minMaxDescriptionAlphaBetaPrune.MinMaxFunc(value, this.GetValueOfBoardAlphaBetaPrune(board, opponent, depth + 1));

                    if (minMaxDescriptionAlphaBetaPrune.Alpha)
                    {
                        this.alpha = Math.Max(alpha, value);
                    }
                    else if (minMaxDescriptionAlphaBetaPrune.Beta)
                    {
                        this.beta = Math.Min(this.beta, value);
                    }

                    board.Set(emptyFieldIdx, Player.None);
                }

                return value;
            }
        }
    }
}
