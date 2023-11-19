
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

        public string BoardFieldsString
        {
            get => this.ToString();
            set => this.SetBoardFields(this.FromString(value));
        }

        public Player StartPlayer { get; set; }

        public override string ToString()
            => string.Join(";", Enumerable.Range(0, 9).Select(field => this.Get(field).AsString()));

        public ThreeXOBoard(string boardString, Player startPlayer) : this()
        {
            this.StartPlayer = startPlayer;
            this.SetBoardFields(Enumerable.Range(0, 9).Select(i => Player.None).ToList());
            var fields = boardString.Split(new char[] { ';' });
            for (int fieldNr = 0; fieldNr < fields.Length; fieldNr++)
            {
                this.Set(fieldNr, fields[fieldNr].PlayerFromString());
            }
        }

        public override int GetHashCode()
        {
            if (!this.hashCode.HasValue)
            {
                this.UpdateValues();
            }

            return this.hashCode.Value;
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

        internal static ThreeXOBoard Empty(Player startPlayer)
        {
            ThreeXOBoard result = new ThreeXOBoard();
            result.StartPlayer = startPlayer;
            result.SetBoardFields(Enumerable.Range(0, 9).Select(i => Player.None).ToList());
            return result;
        }

        internal static ThreeXOBoard Random(Player startPlayer)
        {
            ThreeXOBoard board = new ThreeXOBoard();
            board.StartPlayer = startPlayer;
            var list = Enumerable.Range(0, 9).Select(x => Player.None).ToList();
            list.ForEach(p => p.Random());
            board.SetBoardFields(list);

            return board;
        }

        public IBoard Copy()
        {
            ThreeXOBoard board = ThreeXOBoard.Empty(this.StartPlayer);
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

        public Player Get(int fieldIdx)
            => this.boardFields[fieldIdx];

        public void Set(int fieldNr) => this.Set(fieldNr, this.PlayersTurn());

        public void Set(int fieldNr, Player playerOrComputer)
        {
            this.boardFields[fieldNr] = playerOrComputer;
            this.UpdateValues();
        }

        public bool IsEmpty(int fieldIdx)
            => this.boardFields[fieldIdx] == Player.None;

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

        public bool IsFull() => this.AllFields().All(f => f == Player.Computer || f == Player.Player);

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

        public List<int> AllFieldIdxs() => Enumerable.Range(0, 9).ToList();

        public int FirstDiffIdx(IBoard board)
        {
            var allFieldIdxs = this.AllFieldIdxs();
            foreach(var fieldIdx in allFieldIdxs)
            {
                if (this.Get(fieldIdx) != board.Get(fieldIdx))
                {
                    return fieldIdx;
                } 
            }

            return -1;
        }

        public Player PlayersTurn() => this.PlayersTurn(this.StartPlayer);

        public Player PlayersTurn(Player firstPlayer)
        {
            if (this.boardFields.Count(p => p == firstPlayer) == this.boardFields.Count(p => p == firstPlayer.Opponent()))
            {
                return firstPlayer;
            }
            else
            {
                return firstPlayer.Opponent();
            }
        }

        public bool IsTerminal() => this.IsFull() || this.Winner() != Player.None;

        public double GetValue()
        {
            if (this.Winner() == Player.Player)
            {
                return 1;
            }
            else if (this.Winner() == Player.Computer)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public int NrOfVariants() => this.GetEmptyFieldIdxs().Count();

        private Stack<int> lastSetIdx = new Stack<int>();
        public void SetVariant(int nr)
        {
            this.lastSetIdx.Push(this.GetEmptyFieldIdxs()[nr]);
            this.Set(this.lastSetIdx.Peek(), this.PlayersTurn(this.StartPlayer));            
        }

        public void ReSetVariant() => this.Set(this.lastSetIdx.Pop(), Player.None);

        public IBoardBase GetActVariant() => this;

        private ThreeXOBoard()
        {
        }
    }
}
