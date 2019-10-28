using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTERIX
{
    public class AircraftDB
    {
        public string ICAOAddress { get; set; }
        public string RegID { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Airline { get; set; }
        public string ImageUrl { get; set; }

        public AircraftDB(string[] vs)
        {
            this.ICAOAddress = vs[0];
            this.RegID = vs[1];
            this.Type = vs[2];
            this.Model = vs[3];
            this.Airline = vs[4];
            this.ImageUrl = vs[5];
        }
    }
}
