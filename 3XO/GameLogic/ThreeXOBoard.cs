
namespace TicTacToe.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class ThreeXOBoard : IBoard
    {
        private List<Player> boardFields;

        private int? hashCode = null;
        public override int GetHashCode()
        {
            if (!this.hashCode.HasValue)
            {
                this.UpdateValues();
            }

            return this.hashCode.Value;
        }

        public string BoardFieldsString
        {
            get => this.ToString();
            set
            {
                this.SetBoardFields(this.FromString(value));
            }
        }

        public override string ToString()
            => string.Join(";", Enumerable.Range(0, 9).Select(field => this.Get(field).AsString()));

        public ThreeXOBoard(string boardString) : this()
        {
            this.SetBoardFields(Enumerable.Range(0, 9).Select(i => Player.None).ToList());
            var fields = boardString.Split(new char[] { ';' });
            for (int fieldNr = 0; fieldNr < fields.Length; fieldNr++)
            {
                this.Set(fieldNr, fields[fieldNr].PlayerFromString());
            }
        }

        private void SetBoardFields(List<Player> players)
        {
            this.boardFields = players;
            this.UpdateValues();
        }

        private void UpdateValues()
        {
            this.hashCode = this.CalculateHashCode();
        }

        private int CalculateHashCode()
        {
            int result = 0;

            int xbase = 1;
            for (int x = 0; x < this.boardFields.Count; x++)
            {
                result += xbase * this.boardFields[x].AsInt();
                xbase *= 3;
            }

            return result;
        }

        public List<Player> FromString(string boardString)
        {
            var fields = boardString.Split(new char[] { ';' });
            return fields.Select(s => s.PlayerFromString()).ToList();
        }

        public static bool operator ==(ThreeXOBoard a, ThreeXOBoard b)
            => (!ReferenceEquals(a, null)) ? a.Equals(b) : ReferenceEquals(b, null);

        public static bool operator !=(ThreeXOBoard a, ThreeXOBoard b)
            => !(a == b);

        public override bool Equals(object o)
        {
            if (o != null && o is ThreeXOBoard board)
            {
                return this.hashCode == board.hashCode;
            }

            return false;
        }

        public int NrOfWinOptions(Player player)
        {
            int nrOfWins = 0;
            for (int fieldNr = 0; fieldNr < 9; fieldNr++)
            {
                if (this.Get(fieldNr) == Player.None)
                {
                    this.Set(fieldNr, player);
                    if (this.Winner() == player)
                    {
                        nrOfWins++;
                    }
                    this.Set(fieldNr, Player.None);
                }
            }

            return nrOfWins;
        }

        internal static ThreeXOBoard Empty()
        {
            ThreeXOBoard result = new ThreeXOBoard();
            result.SetBoardFields(Enumerable.Range(0, 9).Select(i => Player.None).ToList());
            return result;
        }

        internal static ThreeXOBoard Random()
        {
            ThreeXOBoard board = new ThreeXOBoard();
            var list = Enumerable.Range(0, 9).Select(x => Player.None).ToList();
            list.ForEach(p => p.Random());
            board.SetBoardFields(list);

            return board;
        }

        public IBoard Copy()
        {
            ThreeXOBoard board = ThreeXOBoard.Empty();
            board.SetBoardFields(new List<Player>(this.boardFields));
            return board;
        }

        internal List<Player> Reihe(int nr)
            => ReiheIndexes(nr).Select(idx => this.Get(idx)).ToList();

        internal List<Player> Spalte(int nr)
            => SpalteIndexes(nr).Select(idx => this.Get(idx)).ToList();

        internal List<Player> DiagonaleLIURO()
            => DiagonaleLIUROIndexes().Select(idx => this.Get(idx)).ToList();

        internal List<Player> DiagonaleLOURU()
            => DiagonaleLOURUIndexes().Select(idx => this.Get(idx)).ToList();

        internal static List<int> ReiheIndexes(int nr)
            => new List<int> { 0 + nr * 3, 1 + nr * 3, 2 + nr * 3 };

        internal static List<int> SpalteIndexes(int nr)
            => new List<int> { 6 + nr, 3 + nr, 0 + nr };

        internal static List<int> DiagonaleLIUROIndexes()
            => new List<int> { 0, 4, 8 };

        internal static List<int> DiagonaleLOURUIndexes()
            => new List<int> { 6, 4, 2, };

        public Player Get(int fieldNr)
            => this.Get(new Coordinates(fieldNr));

        internal Player Get(Coordinates coordinates)
            => this.boardFields[coordinates.FieldNr];

        public void Set(int fieldNr, Player playerOrComputer)
        {
            this.boardFields[fieldNr] = playerOrComputer;
            this.UpdateValues();
        }

        public void Set(ICoordinates coordinates, Player playerOrComputer) => this.Set(coordinates.FieldNr, playerOrComputer);

        public bool IsEmpty(ICoordinates coordinates)
            => this.boardFields[coordinates.FieldNr] == Player.None;

        public List<Player> AllFields()
            => Enumerable.Range(0, 9).Select(idx => this.Get(idx)).ToList();

        public List<int> GetEmptyFieldIdxs()
        {
            List<int> result = new List<int>();

            for (int idx = 0; idx < 9; idx++)
            {
                if (this.boardFields[idx] == Player.None)
                {
                    result.Add(idx);
                }
            }

            return result;
        }

        public List<string> Print()
        {
            string horLine = " " + string.Join(" ", Enumerable.Repeat("-------", 3));
            List<string> output = new List<string>();

            output.Add(horLine);

            for (int row = 2; row > -1; row--)
            {
                var field0row = this.GetField(0, row);
                var field1row = this.GetField(1, row);
                var field2row = this.GetField(2, row);

                var maxLines = Math.Max(Math.Max(field0row.Count, field1row.Count), field2row.Count);

                for (int i = 0; i < maxLines; i++)
                {
                    output.Add($"| {field0row[i]} | {field1row[i]} | {field2row[i]} |");
                }
                output.Add(horLine);
            }


            /*
            output.Add($"2  |{this.GetField(0, 2)}|{this.GetField(1, 2)}|{this.GetField(2, 2)}|");
            output.Add("   -------");
            output.Add($"1  |{this.GetField(0, 1)}|{this.GetField(1, 1)}|{this.GetField(2, 1)}|");
            output.Add("   -------");
            output.Add($"0  |{this.GetField(0, 0)}|{this.GetField(1, 0)}|{this.GetField(2, 0)}|");
            output.Add("   -------");
            output.Add("    0 1 2");
            */
            return output;
        }

        public bool Full() => this.AllFields().All(f => f == Player.Computer || f == Player.Player);

        public Player Winner()
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

        private List<string> GetField(int x, int y)
            => this.boardFields[new Coordinates(x, y).FieldNr].AsStringList();

        public List<int> GetAllFieldIdxs() => Enumerable.Range(0, 9).ToList();

        private ThreeXOBoard()
        {
            //=> this.BoardFields = Enumerable.Range(0, 9).Select(i => Player.None).ToList();
        }
    }
}
