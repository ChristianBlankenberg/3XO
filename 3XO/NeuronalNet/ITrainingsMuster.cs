using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronalNet
{
    public interface ITrainingsMuster
    {
        double[] EingabeVektor { get; set; }
        double[] Targetvektor { get; set; }
        double[] TatsächlicheAusgabe { get; set; }
    }
}
