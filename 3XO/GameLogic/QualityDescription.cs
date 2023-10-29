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
    public class QualityDescription
    {
        public QualityDescription()
        {
        }

        public QualityDescription(Board board)
        {
            this.Board = board;
            this.QualityMatrix = Enumerable.Range(0, 9).Select(i => 0.0).ToList();
        }

        public static List<QualityDescription> GetQualityDexcriptionList(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<QualityDescription>));

            // Declare an object variable of the type to be deserialized.
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                return (List<QualityDescription>)serializer.Deserialize(reader);
            }
        }


        public Board Board { get; set; }

        public List<double> QualityMatrix { get; set; }
    }
}
