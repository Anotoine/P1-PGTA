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
        internal double[] TotalSec { get; set; }
        internal double[] samples { get; set; }
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
            //Calculem probabilitat de UpDate i probabilitat de detecció


            double[] ProbDet = new double[] { 0, 0 }; //només per MA i Stand
            pUD = new double[] { 0, 0, 0 };
            double[] PlaceProb = new double[] { 0, 0, 0 };
            int[] entrades = new int[] { 0, 0, 0 };
            double window = 0;

            ///////22222222222
            //TotalSec = new double[] { 0, 0, 0 };
            //samples = new double[] { 0, 0, 0 };

            //this.Place.Add(-1);
            //int pos = 0;
            //for (int i = 0; i < this.Place.Count - 1; i++)
            //{
            //    if (this.Place[i] != 0) //nomes eem de calcular per zones 1, 2 i 3
            //    {
            //        if (this.Place[i] == this.Place[i + 1]) //no hi ha salt de zona
            //        {
            //            samples[Place[i] - 1] = samples[Place[i] - 1] + 1;
            //        }
            //        else  //salt de zona al i +1. Calculem probabilitats i vegades que s'ha entrat a la zona 
            //        {
            //            samples[Place[i] - 1] = samples[Place[i] - 1] + 1;
            //            TotalSec[Place[i] - 1] = TotalSec[Place[i] - 1] + (this.DateTimes[i] - this.DateTimes[pos]).TotalSeconds + 1;
            //            pos = i + 1; //inici de seguent interval
            //        }
            //    }
            //    else
            //    {
            //        pos = i + 1;
            //    }
            //}

            //this.Place.Remove(-1);
            //////22222222222


            ///111111111
            DateTime start;
            DateTime end;
            List<DateTime> DT = new List<DateTime>();
            List<int> P = new List<int>();
            this.Place.Add(-1);
            for (int i = 0; i < this.Place.Count - 1; i++)
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

                        //1 Prob of Update
                        double Prob = (DT.Count / ((end - start).TotalSeconds + 1)) * 100;
                        PlaceProb[this.Place[i] - 1] = PlaceProb[this.Place[i] - 1] + Prob;
                        entrades[this.Place[i] - 1] = entrades[this.Place[i] - 1] + 1;

                        //ListDTList.Add(DT);
                        //ListPList.Add(P);

                        ////2 Prob of MLAT Detection
                        //if (this.Place[i] == 3) { } //no fem res
                        //else
                        //{
                        //    if (this.Place[i] == 1)
                        //    {
                        //        window = 2;
                        //    }
                        //    else if (this.Place[i] == 2)
                        //    {
                        //        window = 5;
                        //    }
                        //    DateTime d1 = DT[0];
                        //    DateTime d2 = new DateTime();
                        //    int det = 0;
                        //    int ventanas = Convert.ToInt32((end - start).TotalSeconds - 1);

                        //    for (int k = 0; k < ventanas; k++)
                        //    {
                        //        d2 = d1.AddSeconds(window);
                        //        for (int j = 0; j < DT.Count; j++)
                        //        {
                        //            if (DT[j] < d2 && DT[j] >= d1)
                        //            {
                        //                det = det + 1;
                        //                j = DT.Count;
                        //            }
                        //        }
                        //        d1 = d1.AddSeconds(1);
                        //    }
                        //    ProbDet[this.Place[i] - 1] = ProbDet[this.Place[i] - 1] + (det / ventanas) * 100;
                        //}

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

            //for (int i = 0; i < ProbDet.Length; i++)
            //{
            //    if (ProbDet[i] != 0)
            //    {
            //        ProbDet[i] = ProbDet[i] / entrades[i];
            //    }
            //}


            ////borrem ultim element que nomes l'hem utilitzat per recorre la llista
            this.Place.Remove(-1);

            //111111111

        }
    }
}
