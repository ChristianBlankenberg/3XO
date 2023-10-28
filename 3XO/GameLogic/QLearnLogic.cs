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

        internal void QLearn()
        {
            this.boardsFieldPlayer = new Stack<(Board, int, Player)>();
            this.playerBoardsAndQValues = new List<QualityDescription>();
            this.computerBoardsAndQValues = new List<QualityDescription>();

            this.GetQValue(Board.Empty(), Player.Player, -1, 0);

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
    
        private void CheckReward(Board board, Player player)
        {
            if (board.Winner() != Player.None)
            {
                double qValue = 1;
                var boardsFieldPlayerList = this.boardsFieldPlayer.ToList();
                
                for(int boardsFieldPlayerListIdx = 0; boardsFieldPlayerListIdx < boardsFieldPlayerList.Count - 1; boardsFieldPlayerListIdx++)
                {
                    var boardAndQValues = this.GetBoardAndQValues(boardsFieldPlayerList[boardsFieldPlayerListIdx+1]);
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
            boardsFieldPlayer.Push((board, setFieldNr , this.Opponent(player)));
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
