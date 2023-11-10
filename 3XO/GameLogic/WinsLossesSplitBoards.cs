namespace GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using TicTacToe.GameLogic;

    public class WinsLossesSplitBoards
    {
        public WinsLossesSplitBoards()
        {
        }

        public WinsLossesSplitBoards(int depth)
        {
            this.Win = Enumerable.Repeat(0, depth).ToList();
            this.Loose = Enumerable.Repeat(0, depth).ToList();
            this.SplitWinBoard = Enumerable.Repeat(0, depth).ToList();
            this.SplitLooseBoard = Enumerable.Repeat(0, depth).ToList();
        }

        public List<int> Win;
        public List<int> Loose;
        public List<int> SplitWinBoard;
        public List<int> SplitLooseBoard;

        internal void Register(int depth, Player boardWinner, Player player)
        {
            if (boardWinner != Player.None)
            {
                int incDec = boardWinner == player ? 1 : -1;
                List<int> list = boardWinner == player ? this.Win : this.Loose;
                if (list != null)
                {
                    list[depth] += 1;
                }
            }
        }

        internal void RegisterSplitBoard(int depth, int nrOfWinOptions, bool positive)
        {
            if (nrOfWinOptions > 1)
            {
                if (positive)
                {
                    this.SplitWinBoard[depth] += 1;
                }
                else
                {
                    this.SplitLooseBoard[depth] += 1;
                }
            }
        }

        public long SplitLooseBoardsQ()
        {
            long result = 0;
            int factor = 1;
            for (int depth = 8; depth > -1; depth--)
            {
                result = result + factor * (this.SplitLooseBoard[depth]);
                factor *= 10;
            }

            return result;
        }

        public long SplitWinBoardsQ()
        {
            long result = 0;
            int factor = 1;
            for (int depth = 8; depth > -1; depth--)
            {
                result = result + factor * (this.SplitWinBoard[depth]);
                factor *= 10;
            }

            return result;
        }

        public long Q()
        {
            long result = 0;
            int factor = 1;
            for (int depth = 8; depth > -1; depth--)
            {
                result = result + factor * (this.Win[depth] - this.Loose[depth]);
                factor *= 10;
            }

            return result;
        }
    }
}
