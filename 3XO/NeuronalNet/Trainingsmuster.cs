using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NeuronalNet
{
    [Serializable]
    public class Trainingsmuster : ITrainingsMuster
    {
        double[] _eingabevektor;
        public double[] EingabeVektor {
            get => _eingabevektor;
            set => _eingabevektor = value; 
        }
        double[] _targetvektor;
        public double[] Targetvektor {
            get => _targetvektor;
            set => _targetvektor = value;
        }
        double[] _tatsächlicheAusgabe;
        public double[] TatsächlicheAusgabe {
            get => _tatsächlicheAusgabe;
            set => _tatsächlicheAusgabe = value; 
        }

        public Trainingsmuster(double[] eingabe, double[] target)
        {
            _eingabevektor = new double[eingabe.Count()];
            _targetvektor = new double[target.Count()];
            _tatsächlicheAusgabe = new double[target.Count()];
            _eingabevektor = eingabe;
            _targetvektor = target;
        }

        public static string Speichern(string dateiname, List<ITrainingsMuster> liste)
        {
            try
            {
                BinaryFormatter writer = new BinaryFormatter();
                FileStream fs =
                new FileStream(dateiname, FileMode.Create);
                writer.Serialize(fs, liste);
                fs.Flush();
                fs.Close();
                return "gespeichert";
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught: ", e.ToString());
                return "failed";
            }
        }

        public static Trainingsmuster Create(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(file);
            BinaryFormatter reader = new BinaryFormatter();
            FileStream fs = new FileStream(file, FileMode.Open);
            Trainingsmuster instanz =
            (Trainingsmuster)reader.Deserialize(fs);
            fs.Close();
            return instanz;
        }
    }
}
