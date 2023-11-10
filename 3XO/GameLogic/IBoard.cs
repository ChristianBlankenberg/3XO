using System.Collections.Generic;

namespace TicTacToe.GameLogic
{
    public interface IBoard
    {
        bool Full();

        bool IsEmpty(ICoordinates coordinates);

        Player Winner();

        Player Get(int fieldNr);

        void Set(int fieldIdx, Player playerOrComputer);

        List<Player> AllFields();

        List<int> GetAllFieldIdxs();

        List<string> Print();

        List<int> GetEmptyFieldIdxs();

        IBoard Copy();

        int NrOfWinOptions(Player player);
    }
}