namespace NeuronalNet.Data
{
    using System;

    class Sigmoid : IAktivierung
    {
        public double ModifyActivation(double value) => 1 / (1 + Math.Exp(-value));
    }
}
