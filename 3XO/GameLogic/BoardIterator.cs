using System;
using TicTacToe.GameLogic;

namespace GameLogic
{
    public class BoardIterator
    {
        public BoardIterator()
        {
        }
     
        public void Iterate(IBoardBase board, int depth, Action<IBoardBase, int> iteration)
        {
            iteration(board, depth);

            if (board.IsTerminal())
            {
            }
            else
            {
                var nrOfVariants = board.NrOfVariants();

                for (int variante = 0; variante < nrOfVariants; variante++)
                {
                    board.SetVariant(variante);

                    this.Iterate(board, depth+1, iteration);

                    board.ReSetVariant();
                }
            }
        }
    }
}
