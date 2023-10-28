using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.GameLogic;

namespace GameLogic
{
    [Serializable]
    public class QualityDescription
    {
        public QualityDescription()
        {
        }

        public QualityDescription(Board board)
        {
            this.Board = board;
            this.QualityMatrix = Enumerable.Range(0, 9).Select(i => 0.0).ToList();
        }

        public Board Board { get; set; }

        public List<double> QualityMatrix { get; set; }
    }
}
