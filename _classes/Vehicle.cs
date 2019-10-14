using System;
using System.Collections.Generic;

namespace ASTERIX
{
    class Vehicle
    {
        internal List<Point> Positions{ get; set; }
        internal List<DateTime> DateTimes { get; set; }
        internal string Type { get; set; }
        internal int TrackN { get; set; }
        internal string Callsign { get; set; }
        internal string ICAOaddress { get; set; }

        public Vehicle(Message m)
        {
            Positions = new List<Point>();
            DateTimes = new List<DateTime>();

            this.ICAOaddress = m.getAddressICAO();
            this.TrackN = m.getTrackN();
            this.Callsign = m.getCallsign();
            this.Positions.Add(m.getPosition());
            this.DateTimes.Add(m.getTOD());

            if (m.getType() == null)
                this.Type = "Aircraft";
            else
                this.Type = m.getType();
        }

        
        public void AddPoint(Message m)
        {
            this.Positions.Add(m.getPosition());
            this.DateTimes.Add(m.getTOD());
        }
    }
}
