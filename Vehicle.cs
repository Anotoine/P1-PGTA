using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asterix
{
    class Vehicle
    {
        private List<Point> Position;
        private List<DateTime> DateTimes;
        private string Type;
        private int TrackN;
        private string Callsign;
        private string ICAOAdress;

        public Vehicle()
        {

        }

        public List<DateTime> DateTimes1 { get => DateTimes; set => DateTimes = value; }
        public string Type1 { get => Type; set => Type = value; }
        public string Callsign1 { get => Callsign; set => Callsign = value; }
        public string ICAOAdress1 { get => ICAOAdress; set => ICAOAdress = value; }
        internal List<Point> Position1 { get => Position; set => Position = value; }
    }
}
