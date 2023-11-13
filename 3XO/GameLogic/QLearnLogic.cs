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
        private readonly Action<string> logAction;

        public QLearnLogic(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        internal void QLearn(IBoard board, Player startPlayer)
        {
            this.boardAndFieldAndPlayers = new List<BoardAndFieldAndPlayer>();

            this.Log("Add Q Values");
            this.AddQValue(board, startPlayer);

            this.Log("Save Results ");
            this.SaveResults(this.boardAndFieldAndPlayers, "boardsFieldPlayer.xml");
        }

        private void CheckWinLoose(List<BoardAndFieldAndPlayer> boardFieldPlayer, Player playerOnTurn)
        {
            Player boardWinner = boardFieldPlayer.Last().Board.Winner();
            double q = 1;

            var boardFieldPlayerRevers = new List<BoardAndFieldAndPlayer>(boardFieldPlayer);
            boardFieldPlayerRevers.Reverse();

            for (int idx = 0; idx < boardFieldPlayerRevers.Count(); idx++)
            {
                List<double> list = boardWinner == boardFieldPlayerRevers[idx].Player ? boardFieldPlayerRevers[idx].Wins : boardFieldPlayerRevers[idx].Loose;
                list[boardFieldPlayerRevers[idx].FieldNr] += q;
                q *= 0.9;
            }

            //var boardsFieldPlayerList = this.boardsFieldPlayer.ToList();

            //for (int boardsFieldPlayerListIdx = 0; boardsFieldPlayerListIdx < boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx++)
            //{
            //    var boardPlayerField = boardsFieldPlayerList[boardsFieldPlayerListIdx];
            //    int fieldNr = boardPlayerField.FieldNr;
            //    var boardsAndQValues = this.GetPreviousPlayerOrComputerBoardAndQValues(boardPlayerField);

            //    boardsAndQValues.WinsLosses[boardPlayerField.FieldNr].Register(boardsFieldPlayerListIdx, boardWinner, boardPlayerField.Player);
            //}
        }

        //private QualityDescription<BoardType> GetPreviousPlayerOrComputerBoardAndQValues(BoardAndFieldAndPlayer boardFieldPlayer)
        //{
        //    BoardType previousBoard = (BoardType)boardFieldPlayer.Board.Copy();
        //    previousBoard.Set(boardFieldPlayer.FieldNr, Player.None);

        //    return this.GetPlayerOrComputerBoardAndQValues(previousBoard);
        //}

        //private QualityDescription<BoardType> GetPlayerOrComputerBoardAndQValues(BoardType board)
        //{

        //    int indexOfBoard = this.boardsAndQValues.FindIndex(bq => bq.Board == board);
        //    if (indexOfBoard == -1)
        //    {
        //        this.boardsAndQValues.Add(new QualityDescription<BoardType>(board));
        //        indexOfBoard = this.boardsAndQValues.Count - 1;
        //    }

        //    return boardsAndQValues[indexOfBoard];
        //}

        private void AddQValue(IBoard board, Player startPlayer)
        {
            this.iterateFieldsList = new List<BoardAndFieldAndPlayer>();

            this.iterateFieldsList.Add(this.GetBoardAndFieldAndPlayer(board, 5, startPlayer.Opponent()));

            this.IterateFields(
                board,
                startPlayer,
                fullAction: (boardList) =>
                {

                },
                winAction: (boardList, player) =>
                {
                    this.CheckWinLoose(boardList, player);
                },
                splitBoardAction: (boardList, player) =>
                {

                });

            /*
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
            */
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

        private void Log(string content) => this.logAction?.Invoke(content);

        private void SaveResults(List<BoardAndFieldAndPlayer> boardAndFieldAndPlayerList, string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BoardAndFieldAndPlayer>));
            StreamWriter streamWriter = new StreamWriter(fileName);
            xmlSerializer.Serialize(streamWriter, boardAndFieldAndPlayerList);
            streamWriter.Close();
        }

        private List<BoardAndFieldAndPlayer> iterateFieldsList;
        private void IterateFields(
            IBoard board,
            Player playerOnTurn,
            Action<List<BoardAndFieldAndPlayer>> fullAction,
            Action<List<BoardAndFieldAndPlayer>, Player> winAction,
            Action<List<BoardAndFieldAndPlayer>, Player> splitBoardAction)
        {
            if (board.IsFull())
            {
                fullAction(this.iterateFieldsList);
            }
            else if (board.Winner() != Player.None)
            {
                winAction(this.iterateFieldsList, playerOnTurn);
            }
            else
            {
                if (board.NrOfWinOptions(Player.Player) > 1)
                {
                    splitBoardAction(this.iterateFieldsList, Player.Player);
                }
                if (board.NrOfWinOptions(Player.Computer) > 1)
                {
                    splitBoardAction(this.iterateFieldsList, Player.Computer);
                }

                var fieldIdxs = board.AllFieldIdxs();
                foreach (var fieldIdx in fieldIdxs)
                {
                    IBoard newBoard = board.Copy();
                    if (newBoard.Get(fieldIdx) == Player.None)
                    {
                        newBoard.Set(fieldIdx, playerOnTurn);

                        this.iterateFieldsList.Add(this.GetBoardAndFieldAndPlayer(newBoard, fieldIdx, playerOnTurn));
                        this.IterateFields(newBoard, playerOnTurn.Opponent(), fullAction, winAction, splitBoardAction);
                        this.iterateFieldsList.RemoveAt(this.iterateFieldsList.Count - 1);

                        newBoard.Set(fieldIdx, Player.None);
                    }
                }
            }
        }

        List<BoardAndFieldAndPlayer> boardAndFieldAndPlayers;

        private BoardAndFieldAndPlayer GetBoardAndFieldAndPlayer(IBoard board, int fieldIdx, Player playerOnTurn)
        {
            int indexOfBoard = this.boardAndFieldAndPlayers.FindIndex(bq => bq.Board == board);
            if (indexOfBoard > -1)
            {
                return this.boardAndFieldAndPlayers[indexOfBoard];
            }
            
            this.boardAndFieldAndPlayers.Add(new BoardAndFieldAndPlayer(board, fieldIdx, playerOnTurn));
            return this.boardAndFieldAndPlayers.Last();
        }
    }
}
