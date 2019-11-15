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
        //internal List<DateTime> timeMA { get; set; }
        //internal List<DateTime> timeS  {get; set; }
        //internal List<DateTime> timeA  { get; set; }

        //internal List<List<DateTime>> ListDTList { get; set; }
        //internal List<List<int>> ListPList { get; set; }

        //internal double tMA { get; set; }
        //internal double tS { get; set; }
        //internal double tA { get; set; }

        internal double[] pUD { get; set; } //vector qe conte les probabilitats de Up Date per les 3 zones 
        //0 --> MA
        //1 --> Stand
        //2 --> Apron

        //internal double PupdateMA { get; set; }
        //internal double PupdateS { get; set; }
        //internal double PupdateA { get; set; }

        //ESTEL PERFO END

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
            DateTime start;
            DateTime end;
            List<DateTime> DT = new List<DateTime>();
            List<int> P = new List<int>();

            pUD = new double[] { 0, 0, 0 };
            double[] PlaceProb = new double[] { 0, 0, 0 };
            int[] entrades = new int[] { 0, 0, 0 };
            

            this.Place.Add(-1); 
            for (int i = 0; i < this.Place.Count -1; i++)
            {
                if (this.Place[i] != 0) //nomes eem de calcular per zones 1, 2 i 3
                {
                    if (this.Place[i] == this.Place[i + 1]) //no hi ha salt de zona
                    {
                        DT.Add(this.DateTimes[i]);
                        P.Add(this.Place[i]);
                    }
                    else  //salt de zona. Calculem probabilitats i vegades que s'ha entrat a la zona 
                    {
                        DT.Add(this.DateTimes[i]);
                        P.Add(this.Place[i]);
                        start = DT[0];
                        end = DT[DT.Count - 1];
                        double Prob = (DT.Count / ((end - start).TotalSeconds + 1)) * 100;
                        PlaceProb[this.Place[i] - 1] = PlaceProb[this.Place[i] - 1] + Prob;
                        entrades[this.Place[i] - 1] = entrades[this.Place[i] - 1] + 1;

                        //ListDTList.Add(DT);
                        //ListPList.Add(P);

                        DT.Clear();
                        P.Clear();
                    }
                }     
            }

            for (int i = 0; i < pUD.Length; i++)
            {
                if (PlaceProb[i] != 0)
                {
                    pUD[i] = PlaceProb[i] / entrades[i];
                }
            }

            //if (PlaceProb[0] != 0)
            //{
            //    PupdateMA = PlaceProb[0] / entrades[0];
            //}
            //if (PlaceProb[1] != 0)
            //{
            //    PupdateS = PlaceProb[1] / entrades[1];
            //}
            //if (PlaceProb[2] != 0)
            //{
            //    PupdateA = PlaceProb[2] / entrades[2];
            //}

            //borrem ultim element que nomes l'hem utilitzat per recorre la llista
            this.Place.Remove(-1);

        }
    }
}
