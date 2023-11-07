using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TicTacToe.GameLogic;

namespace GameLogic
{
    [Serializable]
    public class QualityDescription<BoardType> where BoardType : class, IBoard
    {
        public QualityDescription()
        {
        }

        public QualityDescription(BoardType board)
        {
            this.Board = board;
            this.WinsLosses = Enumerable.Range(0, 9).Select(i => new WinsLossesSplitBoards(9)).ToList();
        }

        public static List<QualityDescription<BoardType>> GetQualityDexcriptionList(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<QualityDescription<BoardType>>));

            // Declare an object variable of the type to be deserialized.
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                return (List<QualityDescription<BoardType>>)serializer.Deserialize(reader);
            }
        }

        public BoardType Board { get; set; }

        public List<WinsLossesSplitBoards> WinsLosses { get; set; }
    }
}
