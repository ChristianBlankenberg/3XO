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
        List<QualityDescription> playerBoardsAndQValues;
        List<QualityDescription> computerBoardsAndQValues;
        Stack<BoardFieldPlayer> boardsFieldPlayer;
        private readonly Action<string> logAction;

        public QLearnLogic(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        internal void QLearn(Board board, Player player)
        {
            this.boardsFieldPlayer = new Stack<BoardFieldPlayer>();
            this.playerBoardsAndQValues = new List<QualityDescription>();
            this.computerBoardsAndQValues = new List<QualityDescription>();

            this.Log("Add Q Values");
            this.AddQValue(board, player, -1, 0);
            //this.playerBoardsAndQValues = QualityDescription.GetQualityDexcriptionList("playerBoardsAndQValues.xml");
            //this.computerBoardsAndQValues = QualityDescription.GetQualityDexcriptionList("computerBoardsAndQValues.xml");

            this.Log("Add Blocking Values");
            this.AddBlockValues(board, player, -1, 0);

            this.Log("Save Results ");
            this.SaveResults(this.playerBoardsAndQValues, "playerBoardsAndQValues.xml");
            this.SaveResults(this.computerBoardsAndQValues, "computerBoardsAndQValues.xml");
        }

        private void SaveResults(List<QualityDescription> qualityDescriptionList, string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<QualityDescription>));
            StreamWriter streamWriter = new StreamWriter(fileName);
            xmlSerializer.Serialize(streamWriter, qualityDescriptionList);
            streamWriter.Close();
        }

        internal void Test()
        {
            var qualityDescription = this.GetQValuesList(Player.Computer);
            this.IterateAndTest(Board.Empty(), Player.Player, 0, 0, qualityDescription,
                (board, player, qDescription) =>
                {
                    if (player == Player.Player)
                    {
                        if (qDescription.FindIndex(b => b.Board == board) == -1)
                        {
                            File.AppendAllText("ComputerNoBoards.txt", $"{board.ToString()}{Environment.NewLine}");
                        }
                    }
                });
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


        private List<QualityDescription> GetQValuesList(Player playerOrComputer)
        {
            if (playerOrComputer == Player.Player)
            {
                return QualityDescription.GetQualityDexcriptionList("playerBoardsAndQValues.xml");
            }
            else if (playerOrComputer == Player.Computer)
            {
                return QualityDescription.GetQualityDexcriptionList("computerBoardsAndQValues.xml");
            }

            return null;
        }

        private void CheckReward(Board board, Player player)
        {
            if (board.Winner() != Player.None)
            {
#if DODEBUG
                bool debug = board.Winner() != Player.None && board.Get(3) != Player.None;
#endif
                Player boardWinner = board.Winner();
                var boardsFieldPlayerList = this.boardsFieldPlayer.ToList();

#if DODEBUG
                if (debug)
                {
                    File.AppendAllText("Debug.txt", $"----------------------------------------------------------------{Environment.NewLine}");
                    File.AppendAllText("Debug.txt", $"Winner={board.Winner()}{Environment.NewLine}");
                    this.Debug(
                        boardsFieldPlayerList[0].Board,
                        boardsFieldPlayerList[0].Player,
                        boardsFieldPlayerList[0].FieldNr, 
                        1, "Winnerboard");
                }
#endif

                double qValue = 1;
                for (int boardsFieldPlayerListIdx = 0; boardsFieldPlayerListIdx < boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx++)
                {
                    boardsFieldPlayerList[boardsFieldPlayerListIdx].QValue = qValue;
                    qValue *= 0.9;
                }

                for (int boardsFieldPlayerListIdx = 0; boardsFieldPlayerListIdx < boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx++)
                {
                    var boardPlayerField = boardsFieldPlayerList[boardsFieldPlayerListIdx];
                    int fieldNr = boardPlayerField.FieldNr;
                    var boardsAndQValues = this.GetPlayerOrComputerBoardAndQValues(boardPlayerField);
                    double q = boardsFieldPlayerList[boardsFieldPlayerListIdx].QValue;

                    double factor = boardWinner == boardPlayerField.Player ? 1 : -1;
                    boardsAndQValues.QualityMatrix[boardPlayerField.FieldNr] += factor * q;

#if DODEBUG
                    if (debug && fieldNr == 3)
                    {
                        this.Debug(boardsAndQValues.Board, boardPlayerField.Player, boardPlayerField.FieldNr, factor * qValue, "Intermediate");
                        File.AppendAllText("Debug.txt", $"----------------------------------------------------------------{Environment.NewLine}");
                    }
#endif
                    qValue *= 0.9;
                }
            }
            else
            {
                // no winner -> 0 reward -> no need to change q-values
                var boardsFieldPlayerList = this.boardsFieldPlayer.ToList();
                var boardsAndQValues = this.GetPlayerOrComputerBoardAndQValues(boardsFieldPlayerList[0]);
            }
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

        private QualityDescription GetPlayerOrComputerBoardAndQValues(BoardFieldPlayer boardFieldPlayer)
        {
            List<QualityDescription> boardsAndQValues = null;

            if (boardFieldPlayer.Player == Player.Player)
            {
                boardsAndQValues = this.playerBoardsAndQValues;
            }
            else if (boardFieldPlayer.Player == Player.Computer)
            {
                boardsAndQValues = this.computerBoardsAndQValues;
            }
            else
            {
                throw new ArgumentException(nameof(boardFieldPlayer.Player));
            }

            var previousBoard = boardFieldPlayer.Board.Copy();
            previousBoard.Set(boardFieldPlayer.FieldNr, Player.None);

            int indexOfBoard = boardsAndQValues.FindIndex(bq => bq.Board == previousBoard);
            if (indexOfBoard == -1)
            {
                boardsAndQValues.Add(new QualityDescription(previousBoard));
                indexOfBoard = boardsAndQValues.Count - 1;
            }

            return boardsAndQValues[indexOfBoard];
        }

        private void AddQValue(Board board, Player playerSet, int setFieldNr, int layerIdx)
        {
            boardsFieldPlayer.Push(new BoardFieldPlayer(board, setFieldNr, playerSet));
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

        private void AddBlockValues(Board board, Player playerSet, int setFieldNr, int layerIdx)
        {
            if (board.Full() || board.Winner() != Player.None)
            {
            }
            else
            {
                var opponent = playerSet.Opponent();

                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    this.LogCalcIndex(layerIdx, fieldNr);

                    Board newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr) == Player.None)
                    {
                        newBoard.Set(fieldNr, opponent);

                        var addBlockingValueReward = this.GetAddBlockingValueReward(newBoard, fieldNr, playerSet);
                        if (addBlockingValueReward > 0)
                        {
                            List<QualityDescription> boardsAndQValues = opponent == Player.Player ? playerBoardsAndQValues : computerBoardsAndQValues;
                            var boardAndQValue = boardsAndQValues.FirstOrDefault(b => b.Board == board);
                            if (boardAndQValue == null)
                            {
                                throw (new ArgumentException(nameof(boardAndQValue)));
                            }

                            boardAndQValue.QualityMatrix[fieldNr] += addBlockingValueReward;
                        }
                     
                        this.AddBlockValues(newBoard, opponent, fieldNr, layerIdx + 1);
                    }
                }
            }
        }

        private double GetAddBlockingValueReward(Board newBoard, int fieldNr, Player playerSet)
        {
            double result = 0;
            newBoard.Set(fieldNr, playerSet);
            if (newBoard.Winner() == playerSet)
            {
                result = 100;
            }

            newBoard.Set(fieldNr, playerSet.Opponent());
            return result;
        }

        private void Log(string content)
        {
            this.logAction?.Invoke(content);
        }
    }
}
