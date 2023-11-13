using System.Collections.Generic;

namespace TicTacToe.GameLogic
{
    public interface IBoardBase
    {
        bool IsTerminal();

        double GetValue();

        int NrOfVariants();

        void SetVariant(int nr);

        IBoardBase GetActVariant();

        void ReSetVariant();
    }
}