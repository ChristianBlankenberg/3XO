namespace TicTacToe.GameLogic
{
    using System;
    
    public enum Player
    {
        Unknown,

        None,

        Computer,

        Player
    }

    internal static class PlayerExtensions
    {
        internal static Player PlayerFromString(this string playerString)
        {
            if (playerString == " ")
            {
                return Player.None;
            }
            else if (playerString == "O")
            {
                return Player.Computer;
            }
            else if (playerString == "X")
            {
                return Player.Player;
            }

            return Player.Unknown;
        }


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

        internal static int AsInt(this Player player)
        {
            switch (player)
            {
                case Player.Unknown:
                    throw new NotImplementedException();
                case Player.None:
                    return 0;
                case Player.Computer:
                    return 1;
                case Player.Player:
                    return 2;
            }

            throw new NotImplementedException();
        }

        internal static Player FromDouble(this Player player, double val) =>
            val == -1 ? Player.Computer :
                val == 0 ? Player.None :
                    val == 1 ? Player.Player :
                        Player.Unknown;

        internal static Player Opponent(this Player player) => player == Player.Player ? Player.Computer : Player.Player;

        internal static void Random(this Player player)
        {
            Random random = new Random();
            var values = Enum.GetValues(typeof(Player));
            player = (Player)values.GetValue(random.Next(0, values.Length));
        }
    }
}
