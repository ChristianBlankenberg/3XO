using System;

namespace TicTacToe.GameLogic
{
    internal class Coordinates
    {
        internal Coordinates(int x, int y)
        {
            if (x < 0 || x > 2)                
            {
                throw new ArgumentException(nameof(x));
            }

            if (y < 0 || y > 2)
            {
                throw new ArgumentException(nameof(y));
            }

            this.X = x;
            this.Y = y;
        }

        internal Coordinates(int fieldNr)
        {
            if (fieldNr < 0 || fieldNr > 8)
            {
                throw new ArgumentException(nameof(fieldNr));
            }

            this.X = fieldNr % 3;
            this.Y = fieldNr / 3;
        }

        internal int X { get; }

        internal int Y { get; }

        internal int GetFieldNr => this.Y * 3 + this.X;
      
    }
}
