using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.GameLogic
{
    internal class Board
    {
        private Player[,] board;

        internal static Board Empty()
        {
            return new Board();
        }

        internal static Board Random()
        {
            Board board = new Board();
            for (int fieldNr = 0; fieldNr < 9; fieldNr++)
            {
                Coordinates coordinates = new Coordinates(fieldNr);
                board.board[coordinates.X, coordinates.Y] = board.board[coordinates.X, coordinates.Y].Random();
            }

            return board;
        }

        internal Board Copy()
        {
            Board board = new Board();
            for (int fieldNr = 0; fieldNr < 9; fieldNr++)
            {
                Coordinates coordinates = new Coordinates(fieldNr);
                board.board[coordinates.X, coordinates.Y] = this.board[coordinates.X, coordinates.Y];
            }

            return board;
        }

        internal List<Player> Reihe(int nr) => ReiheIndexes(nr).Select(idx => this.Get(idx)).ToList();

        internal List<Player> Spalte(int nr) => SpalteIndexes(nr).Select(idx => this.Get(idx)).ToList();

        internal List<Player> DiagonaleLIURO() => DiagonaleLIUROIndexes().Select(idx => this.Get(idx)).ToList();

        internal List<Player> DiagonaleLOURU() => DiagonaleLOURUIndexes().Select(idx => this.Get(idx)).ToList();

        internal static List<int> ReiheIndexes(int nr) => new List<int> { 0 + nr * 3, 1 + nr * 3, 2 + nr * 3 };

        internal static List<int> SpalteIndexes(int nr) => new List<int> { 6 + nr, 3 + nr, 0 + nr };

        internal static List<int> DiagonaleLIUROIndexes() => new List<int> { 0, 4, 8 };

        internal static List<int> DiagonaleLOURUIndexes() => new List<int> { 6, 4, 2, };

        internal Player Get(int fieldNr) => this.Get(new Coordinates(fieldNr));

        internal Player Get(Coordinates coordinates) => this.board[coordinates.X, coordinates.Y];

        internal void Set(int fieldNr, Player playerOrComputer)
        {
            Coordinates coordinates = new Coordinates(fieldNr);
            this.board[coordinates.X, coordinates.Y] = playerOrComputer;
        }    

        internal void Set(int x, int y, Player playerOrComputer) => this.board[x, y] = playerOrComputer;

        internal bool IsEmpty(int x, int y) => this.board[x, y] == Player.None;

        internal List<Player> Fields() => Enumerable.Range(0, 9).Select(idx => this.Get(idx)).ToList();

        internal List<string> Print()
        {
            List<string> output = new List<string>();
            output.Add("   -------");
            output.Add($"2  |{this.GetField(0, 2)}|{this.GetField(1, 2)}|{this.GetField(2, 2)}|");
            output.Add("   -------");
            output.Add($"1  |{this.GetField(0, 1)}|{this.GetField(1, 1)}|{this.GetField(2, 1)}|");
            output.Add("   -------");
            output.Add($"0  |{this.GetField(0, 0)}|{this.GetField(1, 0)}|{this.GetField(2, 0)}|");
            output.Add("   -------");
            output.Add("    0 1 2");

            return output;
        }

        internal bool Full() => this.Fields().All(f => f == Player.Computer || f == Player.Player);

        internal Player Winner()
        {
            for (int nr = 0; nr < 3; nr++)
            {
                var reihe = this.Reihe(nr);
                if (reihe.All(f => f == Player.Player) || reihe.All(f => f == Player.Computer))
                {
                    return reihe.First();
                }

                var spalte = this.Spalte(nr);
                if (spalte.All(f => f == Player.Player) || spalte.All(f => f == Player.Computer))
                {
                    return spalte.First();
                }
            }

            var diagonale = this.DiagonaleLIURO();
            if (diagonale.All(f => f == Player.Player) || diagonale.All(f => f == Player.Computer))
            {
                return diagonale.First();
            }

            diagonale = this.DiagonaleLOURU();
            if (diagonale.All(f => f == Player.Player) || diagonale.All(f => f == Player.Computer))
            {
                return diagonale.First();
            }

            return Player.None;
        }

        private string GetField(int x, int y) => this.board[x, y].AsString();

        private Board()
        {
            this.board = new Player[3, 3];
            for (int x = 0; x < this.board.GetLength(0); x++)
            {
                for (int y = 0; y < this.board.GetLength(1); y++)
                {
                    this.board[x, y] = Player.None;
                }
            }
        }
    }
}
