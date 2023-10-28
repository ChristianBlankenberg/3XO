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
        private Board board;

        private GameNeuronalNet gameNeuronal;

        internal bool Over()
        {
            if (this.board.Full())
            {
                return true;
            }

            return this.board.Winner() != Player.None;
        }

        internal void Clear() => this.board = Board.Empty();

        internal bool IsEmpty(Coordinates coordinates) => this.board.IsEmpty(coordinates.X, coordinates.Y);

        internal void Set(Coordinates coordinates, Player playerOrComputer) => this.board.Set(coordinates.X, coordinates.Y, playerOrComputer);

        internal void Debug()
        {
            QualityDescription qualityDescription = new QualityDescription(this.board);
            qualityDescription.QualityMatrix[3] = Math.PI;

            List<QualityDescription> qualityDescriptionList = new List<QualityDescription>();
            qualityDescriptionList.Add(qualityDescription);
            qualityDescriptionList.Add(qualityDescription);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<QualityDescription>));
            StreamWriter streamWriter = new StreamWriter(@"C:\temp\QualityDescriptionListTest.xml");
            xmlSerializer.Serialize(streamWriter, qualityDescriptionList);
            streamWriter.Close();
        }

        internal List<string> PrintBoard() => this.board.Print();

        internal string BoardToString() => this.board.ToString();

        internal Game(Board board)
        {
            this.board = board;
            this.gameNeuronal = new GameNeuronalNet();
            this.gameNeuronal.Init();
        }

        internal Coordinates GetCoordinatesFromNeuronalNet() => this.gameNeuronal.GetOutput(this.board);

        public void Test()
        {

        }

        internal Board GetBoard() => this.board;
    }
}
