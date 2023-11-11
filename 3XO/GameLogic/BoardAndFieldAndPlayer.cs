
namespace GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using TicTacToe.GameLogic;
    
    public class BoardAndFieldAndPlayer
    {
        public BoardAndFieldAndPlayer()
        { }

        public BoardAndFieldAndPlayer(
            IBoard board, 
            int fieldNr, 
            Player player)
        {
            this.Board = board;
            this.BoardString = this.Board.ToString();
            this.FieldNr = fieldNr;
            this.Player = player;
            this.Opponent = player.Opponent();

            var allFieldsCount = board.GetAllFieldIdxs().Count;

            this.Wins = new List<double>(Enumerable.Repeat(0.0, allFieldsCount));
            this.Loose = new List<double>(Enumerable.Repeat(0.0, allFieldsCount));
        }

        public string BoardString { get; set; }

        public IBoard Board { get; }

        public int FieldNr { get; set; }

        public Player Player { get; set; }

        public Player Opponent { get; }

        public List<double> Wins { get; }

        public List<double> Loose { get; }
    }
}
