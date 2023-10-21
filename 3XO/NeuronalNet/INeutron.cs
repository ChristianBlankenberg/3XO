using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronalesNetzBackpropagation
{
    interface INeutron
    {
        void Aktivierung(INeutron neutr);
        void Ausgabe(INeutron neutr);
    }
}
