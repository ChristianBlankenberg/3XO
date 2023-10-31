
namespace GameLogic
{
    using TicTacToe.GameLogic;
    
    public class BoardFieldPlayer
    {
        public BoardFieldPlayer(
            Board<PlayerComputer> board, 
            int fieldNr, 
            Player player)
        {
            this.Board = board;
            this.FieldNr = fieldNr;
            this.Player = player;
        }

        public Board<PlayerComputer> Board { get; }

        public int FieldNr { get; }

        public Player Player { get; }

        public double QValue { get; set; }
    }
}
