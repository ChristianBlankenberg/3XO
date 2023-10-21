namespace NeuronalNet
{
    using System.Collections.Generic;

    public interface INeuronalesNetz
    {
        double[] Calculate(double[] eingabevektor);
        double[] GetOutput();
        int Train(int anzahlLernschritte, double lernrate, double toleranz, List<ITrainingsMuster> trainingsMuster);
        void GenerateNeuronalNet(int[] layers);
    }
}
