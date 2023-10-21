namespace NeuronalNet.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Neuron
    {
        private List<Neuron> previousLayerNeurons;

        public double[] Weights { get; set; }

        public double NettoInput { get; private set; }

        public double Aktivierung { get; private set; }

        public double Ausgabe { get; private set; }

        private IAktivierung AktivierungsF{ get; set; }

        private IAusgabe AusgabeF { get; set; }

        public static Neuron AsEingabeNeuron(IAktivierung aktivf, IAusgabe ausf) => new Neuron(new List<Neuron>(), aktivf, ausf);

        public static Neuron AsNonEingabeNeuron(List<Neuron> previousLayerNeurons, IAktivierung aktivf, IAusgabe ausf) => new Neuron(previousLayerNeurons, aktivf, ausf);

        private Neuron(List<Neuron> previousLayerNeurons, IAktivierung aktivf, IAusgabe ausf)
        {
            Random rand = new Random();
         
            this.previousLayerNeurons = previousLayerNeurons;
            this.Weights = previousLayerNeurons.Count > 0 ? new double[previousLayerNeurons.Count].Select(w => (double)rand.Next(-1000, 1000) / 1000).ToArray() : new double[0];
            this.NettoInput = 0;
            this.AktivierungsF = aktivf;
            this.AusgabeF = ausf;
        }

        internal void Input(double value)
        {
            this.NettoInput = value;
            this.Aktivierung = this.AktivierungsF.ModifyActivation(this.NettoInput);
            this.Ausgabe = this.AusgabeF.ModifyOutput(this.Aktivierung);
        }

        internal void Calculate()
        {
            this.NettoInput = 0;

            for (int i = 0; i < this.previousLayerNeurons.Count(); i++)
            {
                this.NettoInput += this.previousLayerNeurons[i].Ausgabe * this.Weights[i];
            }

            this.Aktivierung = this.AktivierungsF.ModifyActivation(this.NettoInput);
            this.Ausgabe = this.AusgabeF.ModifyOutput(this.Aktivierung);
        }
    }
}
