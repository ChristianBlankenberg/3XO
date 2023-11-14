using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe.GameLogic;

namespace GameLogicTests
{
    
    [TestClass]
    public class UnitTest1
    {
 
        [TestMethod]
        public void TestAlphaBetaPruneClassTest()
        {
            AlphaBetaPruneClass alphaBetaPruneClass = new AlphaBetaPruneClass(Player.Computer, Player.Player);

            alphaBetaPruneClass.GetValue(this.GetTestBinTreeNodes2(), Player.Computer);
        }

        private IBoardBase GetTestBinTreeNodes1()
        {
            // https://4.bp.blogspot.com/_nWD8gSvCXFk/TOO5im5skTI/AAAAAAAACpE/aRoFjXx-DFI/s1600/Dibujo1.bmp

            return
            new BinTreeNode(
            new BinTreeNode(
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(4), new BinTreeNode(48)),
            new BinTreeNode(new BinTreeNode(15), new BinTreeNode(25)))
                ,
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(36), new BinTreeNode(23)),
            new BinTreeNode(new BinTreeNode(19), new BinTreeNode(-5))))
                    ,
            new BinTreeNode(
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(-25), new BinTreeNode(11)),
            new BinTreeNode(new BinTreeNode(-46), new BinTreeNode(7)))
                ,
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(45), new BinTreeNode(-9)),
            new BinTreeNode(new BinTreeNode(48), new BinTreeNode(10)))));
        }

        private IBoardBase GetTestBinTreeNodes2()
        {
            // https://www.youtube.com/watch?v=_i-lZcbWkps
            return
            new BinTreeNode(
            new BinTreeNode(
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(10), new BinTreeNode(5)),
            new BinTreeNode(new BinTreeNode(7), new BinTreeNode(11)))
                ,
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(12), new BinTreeNode(8)),
            new BinTreeNode(new BinTreeNode(9), new BinTreeNode(8))))
                    ,
            new BinTreeNode(
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(5), new BinTreeNode(12)),
            new BinTreeNode(new BinTreeNode(11), new BinTreeNode(12)))
                ,
            new BinTreeNode(
            new BinTreeNode(new BinTreeNode(9), new BinTreeNode(8)),
            new BinTreeNode(new BinTreeNode(7), new BinTreeNode(10)))));
        }

    }
}
