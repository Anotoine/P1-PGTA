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
        public double ProbOfMLATdet { get; set; }
        public double PoUDTeo { get; set; }
        public double PoMDTeo { get; set; }

        public ShowPerf(string Z, double PoUMA, double PoUDTeo, double PoMD, double PoMDTeo)
        {
            this.PoUDTeo = PoUDTeo;
            this.PoMDTeo = PoMDTeo;
            this.Zone = Z;
            this.ProbOfUpdate = PoUMA;
            this.ProbOfMLATdet = PoMD;
        }

    }
}
