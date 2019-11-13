using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Ideafix
{
    class Vehicle
    {
        internal List<Point> Positions{ get; set; }
        internal List<DateTime> DateTimes { get; set; }
        internal List<int> Place { get; set; }

        //ESTEL PERFO
        internal List<DateTime> timeMA { get; set; }
        internal List<DateTime> timeS  {get; set; }
        internal List<DateTime> timeA  { get; set; }

        internal double tMA { get; set; }
        internal double tS { get; set; }
        internal double tA { get; set; }

        internal double PupdateMA { get; set; }
        internal double PupdateS { get; set; }
        internal double PupdateA { get; set; }

        internal string Type { get; set; }
        internal int TrackN { get; set; }
        internal string Callsign { get; set; }
        internal string ICAOaddress { get; set; }



        public Vehicle(Message m)
        {
            Positions = new List<Point>();
            DateTimes = new List<DateTime>();
            Place = new List<int>();

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

        public List<Point> GetPointsByDate(DateTime dt)
        {
            List<Point> points = new List<Point>();
            bool exit = false;
            int i = 0;
            while (!exit && i < DateTimes.Count)
            {
                if (DateTime.Compare(DateTimes[i], dt) < 0)
                    points.Add(Positions[i]);
                else if (DateTime.Compare(DateTimes[i], dt) >= 0)
                    exit = true;
                
                i++;
            }
            return points;
        }

        public List<Point> GetPointsByRangeDate(DateTime dtStart, DateTime dtStop)
        {
            List<Point> points = new List<Point>();
            bool exit = false;
            int i = 0;
            while (!exit && i < DateTimes.Count)
            {
                if (DateTime.Compare(DateTimes[i],dtStart) > 0) //if time is after dtStart
                {
                    if (DateTime.Compare(DateTimes[i], dtStop) < 0) // if time is before dtStop
                        points.Add(Positions[i]); //Add to the list
                    else if (DateTime.Compare(DateTimes[i], dtStop) >= 0) //but if time is after dtStop
                        exit = true; //exit
                }
                i++;
            }
            return points;
        }

        public List<DateTime> GetTimesByRangeDate(DateTime dtStart, DateTime dtStop)
        {
            List<DateTime> times = new List<DateTime>();
            bool exit = false;
            int i = 0;
            while (!exit && i < DateTimes.Count)
            {
                if (DateTime.Compare(DateTimes[i], dtStart) > 0) //if time is after dtStart
                {
                    if (DateTime.Compare(DateTimes[i], dtStop) < 0) // if time is before dtStop
                        times.Add(DateTimes[i]); //Add to the list
                    else if (DateTime.Compare(DateTimes[i], dtStop) >= 0) //but if time is after dtStop
                        exit = true; //exit
                }
                i++;
            }
            return times;
        }

        public DateTime getLastTime()
        {
            return this.DateTimes[this.DateTimes.Count - 1];
        }

        public DateTime getFirstTime()
        {
            return this.DateTimes[0];
        }

        public List<Point> GetPositions()
        {
            return this.Positions;
        }


        public void Performance() 
        {
            //List<DateTime> timeMA = new List<DateTime>();
            //List<DateTime> timeS = new List<DateTime>();
            //List<DateTime> timeA = new List<DateTime>();
            timeMA = new List<DateTime>();
            timeS = new List<DateTime>();
            timeA = new List<DateTime>();
 
            //cas fora dins fora dins no contemplat
           //cas dins1 dins2 dins1 no contemplat

            for (int i = 0; i < this.Place.Count; i++) 
            {
                if (this.Place[i] == 1) //Maneuvering Area 0.95
                {
                    timeMA.Add(this.DateTimes[i]);
                }
                if (this.Place[i] == 2) //Stand 0.5
                {
                    timeS.Add(this.DateTimes[i]);
                }
                if (this.Place[i] == 3) //Arpon 0.7
                {
                    timeA.Add(this.DateTimes[i]);
                }
            }

            if (timeMA.Count > 0)
            {
                DateTime start = timeMA[0];
                DateTime end = timeMA[timeMA.Count -1];
                tMA= (end - start).TotalSeconds;
                PupdateMA = (timeMA.Count / (tMA+1)) * 100;
            }

            if (timeS.Count > 0)
            {
                DateTime start = timeS[0];
                DateTime end = timeS[timeS.Count - 1];
                tS = (end - start).TotalSeconds;
                PupdateS = (timeS.Count / (tS+1)) * 100;
            }

            if (timeA.Count > 0)
            {
                DateTime start = timeA[0];
                DateTime end = timeA[timeA.Count - 1];
                tA = (end - start).TotalSeconds;
                PupdateA = (timeA.Count / (tA+1)) * 100;
            }
        }
    }


}
