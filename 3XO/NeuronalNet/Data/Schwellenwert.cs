namespace NeuronalNet.Data
{
    class Schwellenwert : IAusgabe
    {
        public double ModifyOutput(double value) => value > 0.5 ? 1.0 : 0;
    }
}
