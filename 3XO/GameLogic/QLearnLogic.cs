using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TicTacToe.GameLogic;

namespace GameLogic
{
    internal class QLearnLogic
    {
        //List<QualityDescription> playerBoardsAndQValues;
        //List<QualityDescription> computerBoardsAndQValues;
        List<QualityDescription> boardsAndQValues;

        Stack<BoardFieldPlayer> boardsFieldPlayer;
        private readonly Action<string> logAction;

        public QLearnLogic(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        internal void QLearn(Board board, Player player)
        {
            this.boardsFieldPlayer = new Stack<BoardFieldPlayer>();
            this.boardsAndQValues = new List<QualityDescription>();

            this.Log("Add Q Values");
            this.AddQValue(board, player, -1, 0);

            this.Log("Save Results ");
            this.SaveResults(this.boardsAndQValues, "boardsAndQValues.xml");
        }

        private void SaveResults(List<QualityDescription> qualityDescriptionList, string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<QualityDescription>));
            StreamWriter streamWriter = new StreamWriter(fileName);
            xmlSerializer.Serialize(streamWriter, qualityDescriptionList);
            streamWriter.Close();
        }

        private void IterateAndTest(Board board, Player player, int setFieldNr, int layerIdx, List<QualityDescription> qualityDescription, Action<Board, Player, List<QualityDescription>> checkAction)
        {
            if (board.Full() || board.Winner() != Player.None)
            {
            }
            else
            {
                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    Board newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr) == Player.None)
                    {
                        newBoard.Set(fieldNr, player);
                        checkAction(newBoard, player, qualityDescription);
                        this.IterateAndTest(newBoard, player.Opponent(), fieldNr, layerIdx + 1, qualityDescription, checkAction);
                    }
                }
            }
        }

        private void CheckReward(Board board, Player player)
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

            /*
            bool splitBoardFound = false;
            for (int boardsFieldPlayerListIdx = boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx >= 0 && !splitBoardFound; boardsFieldPlayerListIdx--)
            {
                var boardPlayerField = boardsFieldPlayerList[boardsFieldPlayerListIdx];

                int nrOfWinOptions = this.GetNrOfWinOptions(boardPlayerField, boardPlayerField.Player);
                if (nrOfWinOptions > 1)
                {
                    splitBoardFound = true;

                    for (int depth = boardsFieldPlayerListIdx; depth < boardsFieldPlayerList.Count - 1; depth++)
                    {
                        int fieldNr2 = boardsFieldPlayerList[depth].FieldNr;
                        var boardsAndQValues = this.GetPlayerOrComputerBoardAndQValues(boardsFieldPlayerList[depth]);
                        boardsAndQValues.WinsLosses[fieldNr2].RegisterSplitBoard(
                            depth - boardsFieldPlayerListIdx,
                            nrOfWinOptions,
                            depth % 2 == 0);
                    }
                }
            }
            */
        }

        private void Debug(Board board, Player player, int fieldNr, double qValue, string comment)
        {
            File.AppendAllText("Debug.txt", $"{comment}{Environment.NewLine}");
            File.AppendAllText("Debug.txt", $"Player={player}{Environment.NewLine}");
            File.AppendAllText("Debug.txt", $"QValue={qValue}{Environment.NewLine}");
            File.AppendAllText("Debug.txt", $"Field Nr={fieldNr}{Environment.NewLine}");
            File.AppendAllText("Debug.txt", $"Board : {Environment.NewLine}");
            board.Print().ForEach(line => File.AppendAllText("Debug.txt", $"{line}{Environment.NewLine}"));
        }

        private QualityDescription GetPreviousPlayerOrComputerBoardAndQValues(BoardFieldPlayer boardFieldPlayer)
        {

            var previousBoard = boardFieldPlayer.Board.Copy();
            previousBoard.Set(boardFieldPlayer.FieldNr, Player.None);

            return this.GetPlayerOrComputerBoardAndQValues(previousBoard);
        }

        private QualityDescription GetPlayerOrComputerBoardAndQValues(Board board)
        {

            int indexOfBoard = this.boardsAndQValues.FindIndex(bq => bq.Board == board);
            if (indexOfBoard == -1)
            {
                this.boardsAndQValues.Add(new QualityDescription(board));
                indexOfBoard = this.boardsAndQValues.Count - 1;
            }

            return boardsAndQValues[indexOfBoard];
        }

        private void AddQValue(Board board, Player playerSet, int setFieldNr, int layerIdx)
        {
            //File.AppendAllText("ComputerCalcBoards.txt", $"{board.ToString()}{Environment.NewLine}");

            boardsFieldPlayer.Push(new BoardFieldPlayer(board, setFieldNr, playerSet));

            int nrOfWinOptions = board.NrOfWinOptions(playerSet);
            if (nrOfWinOptions > 1)
            {
                var boardFielPlayerList = boardsFieldPlayer.ToList();
                for (int i = 0; i < boardFielPlayerList.Count - 1; i++)
                {
                    int fieldNr2 = boardFielPlayerList[i].FieldNr;
                    var boardsAndQValues = this.GetPlayerOrComputerBoardAndQValues(boardFielPlayerList[i].Board);
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
                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    this.LogCalcIndex(layerIdx, fieldNr);

                    Board newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr) == Player.None)
                    {
                        newBoard.Set(fieldNr, playerSet.Opponent());
                        this.AddQValue(newBoard, playerSet.Opponent(), fieldNr, layerIdx + 1);
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

        private double GetBlockingValueReward(BoardFieldPlayer boardFieldPlayer)
        {
            double reward = 0;
            Board board = boardFieldPlayer.Board;

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

        private int GetNrOfWinOptions(BoardFieldPlayer boardFieldPlayer, Player player)
        {
            return boardFieldPlayer.Board.NrOfWinOptions(player);
        }

        private void Log(string content)
        {
            this.logAction?.Invoke(content);
        }
    }
}
