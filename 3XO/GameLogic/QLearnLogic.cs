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

        //        Dictionary<Board, int, Player>
        Stack<(Board, int, Player)> boardsFieldPlayer;

        //		board.ToString()	" ; ; ; ;X;X; ; ;O"	string


        internal void QLearn(Board board, Player player)
        {
            this.boardsFieldPlayer = new Stack<(Board, int, Player)>();
            this.playerBoardsAndQValues = new List<QualityDescription>();
            this.computerBoardsAndQValues = new List<QualityDescription>();

            this.GetQValue(board, player, -1, 0);

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
                            File.AppendAllText("ComputerNoBoards.txt",$"{board.ToString()}{Environment.NewLine}");
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

        private void CheckReward(Board board, Player player)
        {
            if (board.Winner() != Player.None)
            {
                double qValue = 1;
                var boardsFieldPlayerList = this.boardsFieldPlayer.ToList();

                for (int boardsFieldPlayerListIdx = 0; boardsFieldPlayerListIdx < boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx++)
                {
                    beide ...
                    var boardAndQValues = this.GetBoardAndQValues(boardsFieldPlayerList[boardsFieldPlayerListIdx + 1]);
                    boardAndQValues.QualityMatrix[boardsFieldPlayerList[boardsFieldPlayerListIdx].Item2] += qValue;
                    qValue *= -0.9;
                }
            }
            else
            {
                // no winner -> 0 reward -> no need to change q-values
            }
        }

        private QualityDescription GetBoardAndQValues((Board, int, Player) boardFieldPlayer)
        {
            List<QualityDescription> boardsAndQValues = null;

            if (boardFieldPlayer.Item3 == this.Opponent(Player.Player))
            {
                boardsAndQValues = this.playerBoardsAndQValues;
            }
            else if (boardFieldPlayer.Item3 == this.Opponent(Player.Computer))
            {
                boardsAndQValues = this.computerBoardsAndQValues;
            }

            int indexOfBoard = boardsAndQValues.FindIndex(bq => bq.Board == boardFieldPlayer.Item1);
            if (indexOfBoard == -1)
            {
                boardsAndQValues.Add(new QualityDescription(boardFieldPlayer.Item1.Copy()));
                indexOfBoard = boardsAndQValues.Count - 1;
            }

            return boardsAndQValues[indexOfBoard];
        }

        private Player Opponent(Player player) => player == Player.Player ? Player.Computer : Player.Player;

        private void GetQValue(Board board, Player player, int setFieldNr, int layerIdx)
        {
            boardsFieldPlayer.Push((board, setFieldNr, this.Opponent(player)));
            if (board.Full() || board.Winner() != Player.None)
            {
                this.CheckReward(board, player);

                //var lines = board.Print();

                //foreach (var line in lines)
                //{
                //    File.AppendAllText("3XO.txt", $"{line}{Environment.NewLine}");
                //}

                //File.AppendAllText("3XO.txt", $"{Environment.NewLine}");
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
                        this.GetQValue(newBoard, this.Opponent(player), fieldNr, layerIdx + 1);
                        boardsFieldPlayer.Pop();
                    }
                }
            }
        }
    }
}
