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

        internal void QLearn(Board<PlayerComputer> board, Player player)
        {
            this.boardsFieldPlayer = new Stack<BoardFieldPlayer>();
            this.playerBoardsAndQValues = new List<QualityDescription>();
            this.computerBoardsAndQValues = new List<QualityDescription>();

            this.Log("Calculate Q-Values");
            this.AddQValue(board, player, -1, 0);
            //this.playerBoardsAndQValues = QualityDescription.GetQualityDexcriptionList("playerBoardsAndQValues.xml");
            //this.computerBoardsAndQValues = QualityDescription.GetQualityDexcriptionList("computerBoardsAndQValues.xml");

            this.Log("Calculate Block-Values");
            this.AddBlockValues(board, player, -1, 0);

            this.Log("Save Results");
            this.SaveResults(this.playerBoardsAndQValues, "playerBoardsAndQValues.xml");
            this.SaveResults(this.computerBoardsAndQValues, "computerBoardsAndQValues.xml");

            this.Log("Done...");
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
            this.IterateAndTest(Board<PlayerComputer>.Empty(), Player.Player, 0, 0, qualityDescription,
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

        private void IterateAndTest(Board<PlayerComputer> board, Player player, int setFieldNr, int layerIdx, List<QualityDescription> qualityDescription, Action<Board<PlayerComputer>, Player, List<QualityDescription>> checkAction)
        {
            if (board.Full() || !board.Winner().IsNone())
            {
            }
            else
            {
                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    Board<PlayerComputer> newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr).IsNone())
                    {
                        newBoard.Set(fieldNr, new PlayerComputer(player));
                        checkAction(newBoard, player, qualityDescription);
                        this.IterateAndTest(newBoard, this.Opponent(player), fieldNr, layerIdx + 1, qualityDescription, checkAction);
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

        private void CheckReward(Board<PlayerComputer> board, Player player)
        {
            if (!board.Winner().IsNone())
            {
#if DODEBUG
                bool debug = board.Winner() != Player.None && board.Get(3) != Player.None;
#endif
                PlayerComputer boardWinner = board.Winner();
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

                    double factor = boardWinner.Equals(new PlayerComputer(boardPlayerField.Player)) ? 1 : -1;
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

        private void Debug(Board<PlayerComputer> board, Player player, int fieldNr, double qValue, string comment)
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
            previousBoard.Set(boardFieldPlayer.FieldNr, new PlayerComputer(Player.None));

            int indexOfBoard = boardsAndQValues.FindIndex(bq => bq.Board == previousBoard);
            if (indexOfBoard == -1)
            {
                boardsAndQValues.Add(new QualityDescription(previousBoard));
                indexOfBoard = boardsAndQValues.Count - 1;
            }

            return boardsAndQValues[indexOfBoard];
        }

        private Player Opponent(Player player) => player == Player.Player ? Player.Computer : Player.Player;

        private void AddQValue(Board<PlayerComputer> board, Player playerSet, int setFieldNr, int layerIdx)
        {
            boardsFieldPlayer.Push(new BoardFieldPlayer(board, setFieldNr, playerSet));
            if (board.Full() || !board.Winner().IsNone())
            {
                this.CheckReward(board, playerSet);
            }
            else
            {
                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    if (layerIdx < 3)
                    {
                        string ins = string.Empty;
                        for(int idx = 0;idx<layerIdx;idx++)
                        {
                            ins += " ";
                        }

                        this.Log($"{ins}Calculate field {fieldNr} at layer {layerIdx}");
                    }

                    Board<PlayerComputer> newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr).IsNone())
                    {
                        newBoard.Set(fieldNr, new PlayerComputer(this.Opponent(playerSet)));
                        this.AddQValue(newBoard, this.Opponent(playerSet), fieldNr, layerIdx + 1);
                        boardsFieldPlayer.Pop();
                    }
                }
            }
        }

        private void AddBlockValues(Board<PlayerComputer> board, Player playerSet, int setFieldNr, int layerIdx)
        {
            if (board.Full() || !board.Winner().IsNone())
            {
            }
            else
            {
                var opponent = this.Opponent(playerSet);

                for (int fieldNr = 0; fieldNr < 9; fieldNr++)
                {
                    Board<PlayerComputer> newBoard = board.Copy();
                    Coordinates coordinates = new Coordinates(fieldNr);
                    if (newBoard.Get(fieldNr).IsNone())
                    {
                        newBoard.Set(fieldNr, new PlayerComputer(opponent));

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

        private double GetAddBlockingValueReward(Board<PlayerComputer> newBoard, int fieldNr, Player playerSet)
        {
            double result = 0;
            newBoard.Set(fieldNr, new PlayerComputer(playerSet));
            if (newBoard.Winner().Equals(playerSet))
            {
                result = 100;
            }

            newBoard.Set(fieldNr, new PlayerComputer(this.Opponent(playerSet)));
            return result;
        }

        private void Log(string logContent)
        {
            this.logAction?.Invoke(logContent);
        }
    }
}
