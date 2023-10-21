using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.GameLogic
{
    public class Game
    {
        private Board board;

        private GameNeuronalNet gameNeuronal;

        internal bool Full() => this.board.Fields().All(f => f == Player.Computer || f == Player.Player);

        internal bool Over()
        {
            if (this.Full())
            {
                return true;
            }

            for (int nr = 0; nr < 3; nr++)
            {
                var reihe = this.board.Reihe(nr);
                if (reihe.All(f => f == Player.Player) || reihe.All(f => f == Player.Computer))
                {
                    return true;
                }

                var spalte = this.board.Spalte(nr);
                if (spalte.All(f => f == Player.Player) || spalte.All(f => f == Player.Computer))
                {
                    return true;
                }

                var diagonale = this.board.DiagonaleLIURO();
                if (diagonale.All(f => f == Player.Player) || diagonale.All(f => f == Player.Computer))
                {
                    return true;
                }

                diagonale = this.board.DiagonaleLOURU();
                if (diagonale.All(f => f == Player.Player) || diagonale.All(f => f == Player.Computer))
                {
                    return true;
                }
            }

            return false;
        }

        internal void Clear() => this.board = Board.Empty();

        internal bool IsEmpty(Coordinates coordinates) => this.board.IsEmpty(coordinates.X, coordinates.Y);

        internal void Set(Coordinates coordinates, Player playerOrComputer) => this.board.Set(coordinates.X, coordinates.Y, playerOrComputer);

        internal List<string> PrintBoard() => this.board.Print();

        internal Game(Board board)
        {
            this.board = board;
            this.gameNeuronal = new GameNeuronalNet();
        }

        internal Coordinates GetCoordinatesFromNeuronalNet() => this.gameNeuronal.GetOutput(this.board);
    }
}
