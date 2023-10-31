
namespace TicTacToe.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::GameLogic;

    [Serializable]
    public class Board<T> where T : class, IPlayer, new()
    {
        private List<T> boardFields;

        public string BoardFieldsString
        {
            get => this.ToString();
            set => this.boardFields = this.FromString(value);
        }

        public string ToString() => 
            string.Join(";", Enumerable.Range(0, 9).Select(field => this.Get(field).AsString()));

        public Board(string boardString) : this()
        {
            this.boardFields = this.EmptyBoard();

            var fields = boardString.Split(new char[] { ';' });
            for(int fieldNr = 0;fieldNr < fields.Length; fieldNr++)
            {
                T t = new T();
                t.FromString(fields[fieldNr]);
                this.Set(fieldNr, t);
            }
        }

        public List<T> FromString(string boardString)
        {
            List<T> result = new List<T>();

            var fields = boardString.Split(new char[] { ';' });
            T t = new T();
            for(int i=0;i<fields.Length;i++)
            {
                t.FromString(fields[i]);
                result.Add(t);
            }

            return result;
        }

        public static bool operator == (Board<T> a, Board<T> b)
            => (!ReferenceEquals(a, null)) ? a.Equals(b) : ReferenceEquals(b, null);

        public static bool operator !=(Board<T> a, Board<T> b) 
            => !(a == b);

        public override bool Equals(object o)
        {
            if (o != null && o is Board<T> board)
            {
                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    if (this.Get(fieldNr) != board.Get(fieldNr))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        internal static Board<T> Empty()
        { 
            Board<T> result = new Board<T>();
            result.boardFields = result.EmptyBoard();
            return result;
        }

        internal static Board<T> Random()
        {
            Board<T> board = new Board<T>();
            for (int fieldNr = 0; fieldNr < 9; fieldNr++)
            {
                board.boardFields[fieldNr].Random();
            }

            return board;
        }

        internal Board<T> Copy()
        {
            Board<T> board = Board<T>.Empty();
            for (int fieldNr = 0; fieldNr < 9; fieldNr++)
            {
                board.boardFields[fieldNr] = this.boardFields[fieldNr];
            }

            return board;
        }

        internal List<T> Reihe(int nr) 
            => ReiheIndexes(nr).Select(idx => this.Get(idx)).ToList();

        internal List<T> Spalte(int nr) 
            => SpalteIndexes(nr).Select(idx => this.Get(idx)).ToList();

        internal List<T> DiagonaleLIURO() 
            => DiagonaleLIUROIndexes().Select(idx => this.Get(idx)).ToList();

        internal List<T> DiagonaleLOURU() 
            => DiagonaleLOURUIndexes().Select(idx => this.Get(idx)).ToList();

        internal static List<int> ReiheIndexes(int nr) 
            => new List<int> { 0 + nr * 3, 1 + nr * 3, 2 + nr * 3 };

        internal static List<int> SpalteIndexes(int nr) 
            => new List<int> { 6 + nr, 3 + nr, 0 + nr };

        internal static List<int> DiagonaleLIUROIndexes() 
            => new List<int> { 0, 4, 8 };

        internal static List<int> DiagonaleLOURUIndexes() 
            => new List<int> { 6, 4, 2, };

        internal T Get(int fieldNr) 
            => this.Get(new Coordinates(fieldNr));

        internal T Get(Coordinates coordinates) 
            => this.boardFields[coordinates.FieldNr];

        internal void Set(int fieldNr, T playerOrComputer)
            => this.boardFields[fieldNr] = playerOrComputer;
        
        internal void Set(int x, int y, T playerOrComputer)
            => this.boardFields[new Coordinates(x, y).FieldNr] = playerOrComputer;

        internal bool IsEmpty(int x, int y)
            => this.boardFields[new Coordinates(x, y).FieldNr].IsNone();

        internal List<T> Fields() 
            => Enumerable.Range(0, 9).Select(idx => this.Get(idx)).ToList();

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

        internal bool Full() => this.Fields().All(f => !f.IsNone());

        internal T Winner()
        {
            for (int nr = 0; nr < 3; nr++)
            {
                var reihe = this.Reihe(nr);
                if (reihe.All(f => f.IsA()) || reihe.All(f => f.IsB()))
                {
                    return reihe.First();
                }

                var spalte = this.Spalte(nr);
                if (spalte.All(f => f.IsA()) || spalte.All(f => f.IsB()))
                {
                    return spalte.First();
                }
            }

            var diagonale = this.DiagonaleLIURO();
            if (diagonale.All(f => f.IsA()) || diagonale.All(f => f.IsB()))
            {
                return diagonale.First();
            }

            diagonale = this.DiagonaleLOURU();
            if (diagonale.All(f => f.IsA()) || diagonale.All(f => f.IsB()))
            {
                return diagonale.First();
            }

            T t = new T();
            t.None();

            return t;
        }

        private string GetField(int x, int y)
            => this.boardFields[new Coordinates(x, y).FieldNr].AsString();

        private Board()
        {
            //=> this.BoardFields = Enumerable.Range(0, 9).Select(i => Player.None).ToList();
        }


        private List<T> EmptyBoard()
        {
            List<T> result = new List<T>();
            for (int i = 0; i < 9; i++)
            {
                T t = new T();
                t.None();
                result.Add(t);
            }

            return result;
        }
    }
}
