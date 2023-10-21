namespace NeuronalNet.Data
{
    class Linear_bS : IAktivierung
    {
        public double ModifyActivation(double value)
        {
            if (value < -1)
            {
                return -1.0;
            }
            else if (value > 1)
            {
                return 1.0;
            }
            else
            {
                return value;
            }
        }
    }
}
