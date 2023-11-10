using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TicTacToe.GameLogic;

namespace GameLogic
{
    internal class QLearnLogic<BoardType> where BoardType : class, IBoard 
    {
        List<QualityDescription<BoardType>> boardsAndQValues;

        Stack<BoardAndFieldAndPlayer> boardsFieldPlayer;
        private readonly Action<string> logAction;

        public QLearnLogic()
        {
        }

        public QLearnLogic(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        internal void QLearn(IBoard board, Player player)
        {
            this.boardsFieldPlayer = new Stack<BoardAndFieldAndPlayer>();
            this.boardsAndQValues = new List<QualityDescription<BoardType>>();

            this.Log("Add Q Values");
            this.AddQValue(board, player, -1, 0);

            this.Log("Save Results ");
            this.SaveResults(this.boardsAndQValues, "boardsAndQValues.xml");
        }

        private void SaveResults(List<QualityDescription<BoardType>> qualityDescriptionList, string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<QualityDescription<BoardType>>));
            StreamWriter streamWriter = new StreamWriter(fileName);
            xmlSerializer.Serialize(streamWriter, qualityDescriptionList);
            streamWriter.Close();
        }

        private void IterateAndTest(IBoard board, Player player, int setFieldNr, int layerIdx, List<QualityDescription<BoardType>> qualityDescription, Action<IBoard, Player, List<QualityDescription<BoardType>>> checkAction)
        {
            if (board.Full() || board.Winner() != Player.None)
            {
            }
            else
            {
                var fieldIdxs = board.GetAllFieldIdxs();
                foreach (var fieldIdx in fieldIdxs)
                {
                    IBoard newBoard = board.Copy();
                    if (newBoard.Get(fieldIdx) == Player.None)
                    {
                        newBoard.Set(fieldIdx, player);
                        checkAction(newBoard, player, qualityDescription);
                        this.IterateAndTest(newBoard, player.Opponent(), fieldIdx, layerIdx + 1, qualityDescription, checkAction);
                    }
                }
            }
        }

        private void CheckReward(IBoard board, Player player)
        {
            Player boardWinner = board.Winner();
            var boardsFieldPlayerList = this.boardsFieldPlayer.ToList();

            for (int boardsFieldPlayerListIdx = 0; boardsFieldPlayerListIdx < boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx++)
            {
                var boardPlayerField = boardsFieldPlayerList[boardsFieldPlayerListIdx];
                int fieldNr = boardPlayerField.FieldNr;
                var boardsAndQValues = this.GetPreviousPlayerOrComputerBoardAndQValues(boardPlayerField);

                boardsAndQValues.WinsLosses[boardPlayerField.FieldNr].Register(boardsFieldPlayerListIdx, boardWinner, boardPlayerField.Player);
            }
        }

        private QualityDescription<BoardType> GetPreviousPlayerOrComputerBoardAndQValues(BoardAndFieldAndPlayer boardFieldPlayer)
        {
            BoardType previousBoard = (BoardType)boardFieldPlayer.Board.Copy();
            previousBoard.Set(boardFieldPlayer.FieldNr, Player.None);

            return this.GetPlayerOrComputerBoardAndQValues(previousBoard);
        }

        private QualityDescription<BoardType> GetPlayerOrComputerBoardAndQValues(BoardType board)
        {

            int indexOfBoard = this.boardsAndQValues.FindIndex(bq => bq.Board == board);
            if (indexOfBoard == -1)
            {
                this.boardsAndQValues.Add(new QualityDescription<BoardType>(board));
                indexOfBoard = this.boardsAndQValues.Count - 1;
            }

            return boardsAndQValues[indexOfBoard];
        }

        private void AddQValue(IBoard board, Player playerSet, int setFieldNr, int layerIdx)
        {
            boardsFieldPlayer.Push(new BoardAndFieldAndPlayer(board, setFieldNr, playerSet));

            int nrOfWinOptions = board.NrOfWinOptions(playerSet);
            if (nrOfWinOptions > 1)
            {
                var boardFielPlayerList = boardsFieldPlayer.ToList();
                for (int i = 0; i < boardFielPlayerList.Count - 1; i++)
                {
                    int fieldNr2 = boardFielPlayerList[i].FieldNr;
                    var boardsAndQValues = this.GetPlayerOrComputerBoardAndQValues((BoardType)boardFielPlayerList[i].Board);
                    boardsAndQValues.WinsLosses[fieldNr2].RegisterSplitBoard(
                        i,
                        nrOfWinOptions,
                        i % 2 == 0);
                }
            }

            if (board.Full() || board.Winner() != Player.None)
            {
                this.CheckReward(board, playerSet);
            }
            else
            {
                var fieldIdxs = board.GetAllFieldIdxs();
                foreach (var fieldIdx in fieldIdxs)
                {
                    this.LogCalcIndex(layerIdx, fieldIdx);

                    IBoard newBoard = board.Copy();
                    if (newBoard.Get(fieldIdx) == Player.None)
                    {
                        newBoard.Set(fieldIdx, playerSet.Opponent());
                        this.AddQValue(newBoard, playerSet.Opponent(), fieldIdx, layerIdx + 1);
                        boardsFieldPlayer.Pop();
                    }
                }
            }
        }

        private void LogCalcIndex(int layerIdx, int fieldNr)
        {
            if (layerIdx < 3)
            {
                string ins = string.Empty;
                for (int idx = 0; idx < layerIdx; idx++)
                {
                    ins += " ";
                }

                this.Log($"{ins}Calculate field {fieldNr} at layer {layerIdx}");
            }
        }

        private double GetBlockingValueReward(BoardAndFieldAndPlayer boardFieldPlayer)
        {
            double reward = 0;
            IBoard board = boardFieldPlayer.Board;

            if (board.Winner() == Player.None)
            {
                board.Set(boardFieldPlayer.FieldNr, boardFieldPlayer.Opponent);
                if (board.Winner() == boardFieldPlayer.Opponent)
                {
                    reward = 100;
                }
                board.Set(boardFieldPlayer.FieldNr, boardFieldPlayer.Player);
            }

            return reward;
        }

        private int GetNrOfWinOptions(BoardAndFieldAndPlayer boardFieldPlayer, Player player)
        {
            return boardFieldPlayer.Board.NrOfWinOptions(player);
        }

        private void Log(string content)
        {
            this.logAction?.Invoke(content);
        }
    }
}
