namespace GameLogic
{
    using System;
    using TicTacToe.GameLogic;

    public class AlphaBetaPruneClass : IMinMaxValueLogicClass
    {
        private double alpha;
        private double beta;

        public Player MinPlayer { get; }
        public Player MaxPlayer { get; }

        public AlphaBetaPruneClass(Player minPlayer, Player maxPlayer)
        {
            this.MinPlayer = minPlayer;
            this.MaxPlayer = maxPlayer;
        }

        public double GetValue(IBoardBase board, int depth = 0)
        {
            this.alpha = int.MinValue;
            this.beta = int.MaxValue;

            return this.GetValueAlphaBetaPrune(board, depth, alpha, beta);
        }

        private double GetValueAlphaBetaPrune(IBoardBase board, int depth, double alpha, double beta)
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
                        value = Math.Max(value, this.GetValueAlphaBetaPrune(board.GetActVariant(), depth + 1, alpha, beta));
                        alpha = Math.Max(alpha, value);
                    }
                    else if (player == this.MinPlayer)
                    {
                        // Min
                        value = Math.Min(value, this.GetValueAlphaBetaPrune(board.GetActVariant(), depth + 1, alpha, beta));
                        beta = Math.Min(beta, value);
                    }

                    board.ReSetVariant();

                    if (alpha >= beta)
                    {
                        break;
                    }
                }

                return value;
            }
        }
    }

}
