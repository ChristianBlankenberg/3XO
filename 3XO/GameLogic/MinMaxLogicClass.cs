
namespace GameLogic
{
    using System;
    using TicTacToe.GameLogic;

    public class MinMaxLogicClass : IMinMaxValueLogicClass
    {
        private readonly Random random = new Random();

        public Player MinPlayer { get; }
        public Player MaxPlayer { get; }

        public MinMaxLogicClass(Player minPlayer, Player maxPlayer)
        {
            this.MinPlayer = minPlayer;
            this.MaxPlayer = maxPlayer;
        }

        public  double GetValue(IBoardBase board, int depth = 0)
        {
            if (board.IsTerminal())
            {
                return board.GetValue();
            }
            else
            {
                Player player = board.PlayersTurn();
                double value = player == this.MaxPlayer ? double.MinValue : player == this.MinPlayer ? double.MaxValue : 0;
                var nrOfVariants = board.NrOfVariants();

                for (int variante = 0; variante < nrOfVariants; variante++)
                {
                    board.SetVariant(variante);

                    if (player == this.MaxPlayer)
                    {
                        // Max
                        value = Math.Max(value, this.GetValue(board.GetActVariant(), depth + 1));
                    }
                    else if (player == this.MinPlayer)
                    {
                        // Min
                        value = Math.Min(value, this.GetValue(board.GetActVariant(), depth + 1));
                    }

                    board.ReSetVariant();
                }

                return value;
            }
        }
    }
}

