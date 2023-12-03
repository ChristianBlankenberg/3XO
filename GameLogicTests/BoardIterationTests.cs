namespace GameLogicTests
{
    using System.Collections.Generic;
    using GameLogic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TicTacToe.GameLogic;

    [TestClass]
    public class BoardIterationTests
    {
        [TestMethod]
        public void TestIteration()
        {
            BoardIterator boardIterator = new BoardIterator();

            List<string> boardStrings = new List<string>();

            boardIterator.Iterate(
                ThreeXOBoard.Empty(Player.Player), 
                0, 
                (board, depth) =>
                {
                    if (depth == 2)
                    {
                        boardStrings.Add(board.ToString());
                    }
                },
                (board, depth) =>
                {
                });

        }
    }
}
