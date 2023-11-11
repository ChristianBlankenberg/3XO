using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GameLogic;

namespace TicTacToe.GameLogic
{
    public class Game
    {
        private IBoard board;

        private GameNeuronalNet gameNeuronal;

        internal bool Over()
        {
            if (this.board.IsFull())
            {
                return true;
            }

            return this.board.Winner() != Player.None;
        }

        internal void SetBoard(IBoard board) => this.board = board;

        internal void Clear() => this.board = ThreeXOBoard.Empty();

        internal bool IsEmpty(ICoordinates coordinates) => this.board.IsEmpty(coordinates);

        internal void Set(int fieldIdx, Player playerOrComputer) => this.board.Set(fieldIdx, playerOrComputer);

        //internal void Debug()
        //{
        //    QualityDescription qualityDescription = new QualityDescription(this.board);
        //    //qualityDescription.QualityMatrix[3] = Math.PI;

        //    List<QualityDescription> qualityDescriptionList = new List<QualityDescription>();
        //    qualityDescriptionList.Add(qualityDescription);
        //    qualityDescriptionList.Add(qualityDescription);

        //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<QualityDescription>));
        //    StreamWriter streamWriter = new StreamWriter(@"C:\temp\QualityDescriptionListTest.xml");
        //    xmlSerializer.Serialize(streamWriter, qualityDescriptionList);
        //    streamWriter.Close();
        //}

        internal List<string> PrintBoard() => this.board.Print();

        internal string BoardToString() => this.board.ToString();

        internal Game(ThreeXOBoard board)
        {
            this.board = board;
            this.gameNeuronal = new GameNeuronalNet();
            this.gameNeuronal.Init();
        }

        internal Coordinates GetCoordinatesFromNeuronalNet() => this.gameNeuronal.GetOutput(this.board);

        public void Test()
        {

        }

        internal IBoard GetBoard() => this.board;
    }
}
