namespace NeuronalNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NeuronalNet.Data;

    public class NeuronalesNetz : INeuronalesNetz
    {
        //Liste von Listen von Neuronen: Das Netz
        private List<List<Neuron>> Neurons { get; set; }

        //Ausgabe des Netzes aus Eingangsvektor von Doubles erzeugen (Vorwärts propagieren)
        public double[] Calculate(double[] eingabevektor)
        {
            //NettoInput + Ausgabe der Eingabeneuronen setzen;                                
            var firstLayer = Neurons[0];
            for (int i = 0; i < firstLayer.Count(); i++)
            {
                firstLayer[i].Input(eingabevektor[i]);
            }

            //Weitere Schichten bearbeiten

            for (int i = 1; i < this.Neurons.Count(); i++)
            {
                var layer = this.Neurons[i];
                for (int j = 0; j < layer.Count(); j++)
                {
                    layer[j].Calculate();
                }
            }

            return this.GetOutput();
        }

        public double[] GetOutput() => this.Neurons.Last().Select(n => n.Ausgabe).ToArray();

        //Trainieren des Netzes durch Backpropagation (Fehlerrückführung)
        //gibt 1 bei Erfolg zurück (Erfolg heißt: Training beendet unter maximal-Anzahl von Schritten)
        public int Train(int anzahlLernschritte, double lernrate, double toleranz, List<ITrainingsMuster> trainingsMuster)
        {
            int count = 0;

            bool trained = false;

            for (int schritte = 0; schritte < anzahlLernschritte && !trained; schritte++)
            {
                count++;
                //List<List<double>> netzdeltas = new List<List<double>>();
                trained = true;
                for (int tm = 0; tm < trainingsMuster.Count(); tm++)
                {
                    List<List<double>> netzdeltas = new List<List<double>>();
                    double[] ausgabe = Calculate(trainingsMuster[tm].EingabeVektor);
                    //Ausgabe vergleichen
                    int aus_s = Neurons.Count() - 1;//"Index" der Ausgabeschicht
                    int aus_n = Neurons[aus_s].Count();//Anzahl Ausgabeneuronen
                    List<double> deltas = new List<double>();

                    for (int aus = 0; aus < aus_n; aus++)
                    {
                        //Ableitung der Aktivierungsfunktion nutzen (o*(1-o))
                        double diff = Neurons[aus_s][aus].Aktivierung * (1 - Neurons[aus_s][aus].Aktivierung);
                        double this_delta = diff * (trainingsMuster[tm].Targetvektor[aus] - Neurons[aus_s][aus].Aktivierung);
                        deltas.Add(this_delta);
                        if (Math.Abs(this_delta) > toleranz)
                        {
                            trained = false;
                        }
                    }
                    netzdeltas.Add(deltas);
                    //wenn ein Delta der Ausgabeschicht ausserhalb der Toleranz war, wird jetzt trainiert
                    //=>Fehlerrückführung
                    if (!trained)
                    {
                        //Gewichtsänderung: /\W|u->p = Lernrate * Delta|u * out|p MIT u: betrachtetes Neuron, p: predecessor

                        //Fehlerrückübertragung: Delta|u = SUMME(Delta|s * W|s->u) * Lambda|u MIT s: successor
                        int p = 0;
                        for (int i = Neurons.Count() - 2; i > 0; i--)
                        {
                            List<double> deltas_i = new List<double>();
                            for (int j = 0; j < Neurons[i].Count(); j++)
                            {
                                double abl = Neurons[i][j].Ausgabe * (1 - Neurons[i][j].Ausgabe);
                                double sum = 0.0;

                                for (int z = 0; z < Neurons[i + 1].Count(); z++)
                                {
                                    sum += netzdeltas[p][z] * this.Neurons[i + 1][z].Weights[j];
                                }
                                deltas_i.Add(abl * sum);
                            }
                            netzdeltas.Add(deltas_i);
                            p++;
                        }

                        //Deltsa-Liste umkehren, um selbe Struktur zu haben wie Neuronen-Liste
                        netzdeltas.Reverse();

                        //Gewichtswert-Deltas

                        for (int i = Neurons.Count() - 1; i > 0; i--)
                        {
                            for (int j = 0; j < Neurons[i].Count(); j++)
                            {
                                //jeweiliger Fehler aus umgekehrter Deltaliste
                                double delta = netzdeltas[i - 1][j];

                                for (int z = 0; z < Neurons[i - 1].Count(); z++)
                                {

                                    //jeweiliger Gewichtsmatrixeintrag
                                    double gme = this.Neurons[i][j].Weights[z];
                                    double gewichtsdelta = lernrate * delta * Neurons[i - 1][z].Ausgabe;
                                    this.Neurons[i][j].Weights[z] = gme + gewichtsdelta;
                                }

                            }
                        }
                    }

                }
            }

            return trained ? count : -1; //-1: Training nicht erfolgreich
        }

        //Generiert ein Netz aus einer variablen Anzahl von Schichten
        //anzahlschichten: Anzahl der Schichten (muss mindestens 2 sein)
        //neuronenProSchicht: Anzahl der Neuronen pro Schicht, in Vektor gespeichert
        //Anzahl der Neuronen pro Schicht darf nicht null sein
        public void GenerateNeuronalNet(int[] layers)
        {
            //Neuronen erstellen, in Liste einfügen
            this.Neurons = new List<List<Neuron>>();
            for (int layerIdx = 0; layerIdx < layers.Length; layerIdx++)
            {
                List<Neuron> neuronen = new List<Neuron>();
                for (int idx = 0; idx < layers[layerIdx]; idx++)
                {
                    Neuron neuron;
                    if (layerIdx == 0)
                    {
                        neuron = Neuron.AsEingabeNeuron(new Sigmoid(), new AusgabeNormal());
                    }
                    else if (layerIdx < layers.Length - 1)
                    {
                        neuron = Neuron.AsNonEingabeNeuron(this.Neurons[layerIdx - 1], new Sigmoid(), new AusgabeNormal());
                    }
                    else
                    {
                        neuron = Neuron.AsNonEingabeNeuron(this.Neurons[layerIdx - 1], new Sigmoid(), new Schwellenwert());
                    }

                    neuronen.Add(neuron);
                }
                this.Neurons.Add(neuronen);
            }
        }
    }
}
