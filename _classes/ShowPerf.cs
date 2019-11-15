using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ideafix
{
    public class ShowPerf
    {
        public string Zone { get; set; }
        public double ProbOfUpdate { get; set; }

        public ShowPerf(string Z, double PoUMA)
        {
            this.Zone = Z;
            this.ProbOfUpdate = PoUMA;
        }

    }
}
