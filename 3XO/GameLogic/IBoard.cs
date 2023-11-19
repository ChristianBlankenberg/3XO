using System.Collections.Generic;

namespace TicTacToe.GameLogic
{
    public interface IBoard : IBoardBase
    {
        bool IsFull();

        Player Winner();

        bool IsEmpty(int fieldIdx);

        List<int> AllFieldIdxs();

        Player Get(int fieldIdx);

        void Set(int fieldIdx);

        void Set(int fieldIdx, Player player);

        List<Player> AllFields();

        List<string> Print();

        List<int> GetEmptyFieldIdxs();

        IBoard Copy();

        int NrOfWinOptions(Player player);

        int FirstDiffIdx(IBoard board);

        Player PlayersTurn(Player firstPlayer);

    }
}