using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.GameLogic
{
    public class Game
    {
        private Board board;

        private GameNeuronalNet gameNeuronal;

        internal bool Over()
        {
            if (this.board.Full())
            {
                return true;
            }

            return this.board.Winner() != Player.None;
        }

        internal void Clear() => this.board = Board.Empty();

        internal bool IsEmpty(Coordinates coordinates) => this.board.IsEmpty(coordinates.X, coordinates.Y);

        internal void Set(Coordinates coordinates, Player playerOrComputer) => this.board.Set(coordinates.X, coordinates.Y, playerOrComputer);

        internal List<string> PrintBoard() => this.board.Print();

        internal string BoardToString() => this.board.ToString();

        internal Game(Board board)
        {
            this.board = board;
            this.gameNeuronal = new GameNeuronalNet();
            this.gameNeuronal.Init();
        }

        internal Coordinates GetCoordinatesFromNeuronalNet() => this.gameNeuronal.GetOutput(this.board);

        public void Test()
        {

        }
    }
}
