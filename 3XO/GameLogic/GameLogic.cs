namespace TicTacToe.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::GameLogic;

    public static class GameLogic
    {
        private static Random random = new Random();

        public static int GetMaxValueFieldIdx(IBoard board, IMinMaxValueLogicClass minMaxValueLogicClass)
        {
            if (!board.IsTerminal())
            {
                Player player = board.PlayersTurn();
                var emptyFieldIdxs = board.GetEmptyFieldIdxs();
                List<(int idx, double value)> idxAndValue = new List<(int idx, double value)>();

                foreach (var emptyFieldIdx in emptyFieldIdxs)
                {
                    board.Set(emptyFieldIdx, player);
                    idxAndValue.Add((emptyFieldIdx, minMaxValueLogicClass.GetValue(board, 0)));
                    board.Set(emptyFieldIdx, Player.None);
                }

                var minMaxValue = player == minMaxValueLogicClass.MaxPlayer ? idxAndValue.Max(x => x.value) : player == minMaxValueLogicClass.MinPlayer ? idxAndValue.Min(x => x.value) : 0;
                var minMaxValuesAndIdxs = idxAndValue.Where(x => x.value == minMaxValue).ToList();

                return minMaxValuesAndIdxs[random.Next(0, minMaxValuesAndIdxs.Count)].idx;
            }

            return -1;
        }
    }
}
