
namespace GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TicTacToe.GameLogic;
    
    public class GameLogicMinMax
    {
        private readonly Random random = new Random();
        private readonly Player minPlayer;
        private readonly Player maxPlayer;

        public GameLogicMinMax(Player minPlayer, Player maxPlayer)
        {
            this.minPlayer = minPlayer;
            this.maxPlayer = maxPlayer;
        }

        public int GetFavouriteFieldIdx(IBoard board, Player player)
        {
            var emptyFieldIdxs = board.GetEmptyFieldIdxs();
            List<(int idx, double value)> idxAndValue = new List<(int idx, double value)>(); 

            foreach(var emptyFieldIdx in emptyFieldIdxs)
            {
                board.Set(emptyFieldIdx, player);
                idxAndValue.Add((emptyFieldIdx, this.GetValue(board, 0, player.Opponent())));
                board.Set(emptyFieldIdx, Player.None);
            }

            var minMaxValue = player == this.maxPlayer ? idxAndValue.Max(x => x.value) : player == this.minPlayer ? idxAndValue.Min(x => x.value) : 0;
            var minMaxValuesAndIdxs = idxAndValue.Where(x => x.value == minMaxValue).ToList();            

            return minMaxValuesAndIdxs[random.Next(0, minMaxValuesAndIdxs.Count)].idx;
        }

        private double GetValue(IBoardBase binTreeNode, int depth, Player player)
        {
            if (binTreeNode.IsTerminal())
            {
                return binTreeNode.GetValue();
            }
            else
            {
                double value = player == this.maxPlayer ? double.MinValue : player == this.minPlayer ? double.MaxValue : 0;
                var nrOfVariants = binTreeNode.NrOfVariants();

                for (int variante = 0; variante < nrOfVariants; variante++)
                {
                    binTreeNode.SetVariant(variante);

                    if (player == this.maxPlayer)
                    {
                        // Max
                        value = Math.Max(value, this.GetValue(binTreeNode.GetActVariant(), depth + 1, player.Opponent()));
                    }
                    else if (player == this.minPlayer)
                    {
                        // Min
                        value = Math.Min(value, this.GetValue(binTreeNode.GetActVariant(), depth + 1, player.Opponent()));
                    }

                    binTreeNode.ReSetVariant();
                }

                return value;
            }
        }
    }
}

