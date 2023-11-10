
namespace GameLogic
{
    using TicTacToe.GameLogic;
    
    public class BoardAndFieldAndPlayer
    {
        public BoardAndFieldAndPlayer(
            IBoard board, 
            int fieldNr, 
            Player player)
        {
            this.Board = board;
            this.FieldNr = fieldNr;
            this.Player = player;
            this.Opponent = player.Opponent();
        }

        public IBoard Board { get; }

        public int FieldNr { get; }

        public Player Player { get; }

        public Player Opponent { get; }

        //public double QValue { get; set; }
    }
}
