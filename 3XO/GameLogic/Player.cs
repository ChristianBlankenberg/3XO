using System;

namespace TicTacToe.GameLogic
{
    internal enum Player
    {
        Unknown,

        None,

        Computer,

        Player
    }

    internal static class PlayerExtensions
    {
        internal static string AsString(this Player player)
        {
            switch (player)
            {
                case Player.Unknown:
                    return "?";
                case Player.None:
                    return " ";
                case Player.Computer:
                    return "O";
                case Player.Player:
                    return "X";
            }

            return "?";
        }

        internal static double AsDouble(this Player player)
        {
            switch (player)
            {
                case Player.Unknown:
                    throw new NotImplementedException();
                case Player.None:
                    return 0;
                case Player.Computer:
                    return -1;
                case Player.Player:
                    return 1;
            }

            throw new NotImplementedException();
        }

        internal static Player FromDouble(this Player player, double val) =>
            val == -1 ? Player.Computer :
                val == 0 ? Player.None :
                    val == 1 ? Player.Player :
                        Player.Unknown;

        internal static Player Random(this Player player)
        {
            Random random = new Random();
            var values = Enum.GetValues(typeof(Player));
            return (Player)values.GetValue(random.Next(0, values.Length));
        }
    }
}
