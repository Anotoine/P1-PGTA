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
        internal double[] incSamples { get; set; }
        internal double[] winOK { get; set; }
        internal double[] winT { get; set; }
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
            this.Positions.Add(m.getPositionXY());
            this.DateTimes.Add(m.getTOD());

            if (m.getType() == null)
                this.Type = "Aircraft";
            else
                this.Type = m.getType();
        }

        public void AddPoint(Message m)
        {
            this.Positions.Add(m.getPositionXY());
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
            /////MLAT aixo es un intent fracasat del Papa
            //int oldPos = 0;
            //double Len = this.Place.Count;
            //int i = 0;
            //int x = 0;
            //int w = 0;
            //winOK = new double[] { 0, 0 };
            //winT = new double[] { 0, 0 };
            //DateTime t0 = new DateTime();
            //t0 = this.DateTimes[0];
            //while (i < Len)
            //{
            //    if (this.Place[i] == 1 || this.Place[i] == 2)
            //    {
            //        oldPos = this.Place[i];
            //        if (oldPos == 1) { w = 2; } else { w = 5; }
            //        while (this.Place[i] == oldPos)
            //        {
            //            x = 0;
            //            while (((this.DateTimes[i] - t0).TotalSeconds < w) && this.Place[i] == oldPos && (i + w - 1) < Len)
            //            {
            //                x++; i++;
            //            }
            //            winT[oldPos - 1]++;
            //            if (x > 0)
            //            {
            //                winOK[oldPos - 1]++;
            //            }
            //            t0 = t0.AddSeconds(1);
            //            i = 0;
            //            while (this.DateTimes[i] < t0 && i < Len && (this.Place[i] < 1 || this.Place[i] > 2))
            //            {
            //                i++;
            //            }
            //            oldPos = this.Place[i--];
            //        }
            //    }
            //    i++;
            //}
            /////


            //MLAT det start només per zones 1 i 2
            winOK = new double[] { 0, 0 };
            winT = new double[] { 0, 0 };
            double w = 1;
            this.Place.Add(-1);
            DateTime d0 = new DateTime();
            DateTime d1 = new DateTime();
            DateTime d2 = new DateTime();
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            start = this.DateTimes[0];

            this.DateTimes.Add(d0);
            d1 = start;
            double punt = 0;
            for (int i = 0; i < this.Place.Count - 1; i++)
            {
                if (this.Place[i] == 1 || this.Place[i] == 2) //nomes eem de calcular per zones 1 i 2
                {
                    if (this.Place[i] == this.Place[i + 1]) { } //no hi ha salt de zona
                    else  //salt de zona al i +1.
                    {
                        if (this.Place[i] == 1) { w = 2; } //finestra temporal de 2s
                        if (this.Place[i] == 2) { w = 5; } //finestra temporal de 5s
                        d2 = d1.AddSeconds(w);
                        end = this.DateTimes[i];
                        if (w <= (end - start).TotalSeconds)
                        {
                            int windows = Convert.ToInt32((end - start).TotalSeconds - w + 2);
                            winT[this.Place[i] - 1] = winT[this.Place[i] - 1] + windows;
                            for (int j = 0; j < windows; j++)
                            {
                                for (int k = 0; k < this.DateTimes.Count; k++)
                                {
                                    if (DateTimes[k] >= d1 && DateTimes[k] < d2)
                                    {
                                        winOK[this.Place[i] - 1]++;
                                        k = this.DateTimes.Count;
                                    }
                                }
                                d2 = d1.AddSeconds(w);
                                d1 = d1.AddSeconds(1);
                            }
                        }
                        start = this.DateTimes[i + 1];
                        d1 = start;
                    }
                }
                else
                {
                    start = this.DateTimes[i + 1];
                    d1 = start;
                }
            }
            this.Place.Remove(-1);
            this.DateTimes.Remove(d0);
            //MLAR det end

            /////UPDATE 22222222222
            TotalSec = new double[] { 0, 0, 0 };
            incSamples = new double[] { 0, 0, 0 };

            this.Place.Add(-1);
            int pos = 0;
            for (int i = 0; i < this.Place.Count - 1; i++)
            {
                if (this.Place[i] != 0) //nomes eem de calcular per zones 1, 2 i 3
                {
                    if (this.Place[i] == this.Place[i + 1]) //no hi ha salt de zona
                    {
                        incSamples[Place[i] - 1]++;
                    }
                    else  //salt de zona al i +1. Calculem probabilitats i vegades que s'ha entrat a la zona 
                    {
                        TotalSec[Place[i] - 1] = TotalSec[Place[i] - 1] + (this.DateTimes[i] - this.DateTimes[pos]).TotalSeconds;
                        pos = i + 1; //inici de seguent interval
                    }
                }
                else
                {
                    pos = i + 1;
                }
            }
            this.Place.Remove(-1);
            ////UPDATE 22222222222





            /////UPDATE 111111111
            //pUD = new double[] { 0, 0, 0 };
            //double[] PlaceProb = new double[] { 0, 0, 0 };
            //int[] entrades = new int[] { 0, 0, 0 };
            //DateTime start;
            //DateTime end;
            //List<DateTime> DT = new List<DateTime>();
            //List<int> P = new List<int>();
            //this.Place.Add(-1);
            //for (int i = 0; i < this.Place.Count - 1; i++)
            //{
            //    if (this.Place[i] != 0) //nomes eem de calcular per zones 1, 2 i 3
            //    {
            //        if (this.Place[i] == this.Place[i + 1]) //no hi ha salt de zona
            //        {
            //            DT.Add(this.DateTimes[i]);
            //            P.Add(this.Place[i]);
            //        }
            //        else  //salt de zona. Calculem probabilitats i vegades que s'ha entrat a la zona 
            //        {
            //            DT.Add(this.DateTimes[i]);
            //            P.Add(this.Place[i]);
            //            start = DT[0];
            //            end = DT[DT.Count - 1];

            //            //1 Prob of Update
            //            double Prob = (DT.Count / ((end - start).TotalSeconds + 1)) * 100;
            //            PlaceProb[this.Place[i] - 1] = PlaceProb[this.Place[i] - 1] + Prob;
            //            entrades[this.Place[i] - 1] = entrades[this.Place[i] - 1] + 1;


            //            DT.Clear();
            //            P.Clear();
            //        }
            //    }
            //}

            //for (int i = 0; i < pUD.Length; i++)
            //{
            //    if (PlaceProb[i] != 0)
            //    {
            //        pUD[i] = PlaceProb[i] / entrades[i];
            //    }
            //}

            //////borrem ultim element que nomes l'hem utilitzat per recorre la llista
            //this.Place.Remove(-1);

            ////UPDATE111111111

        }
    }
}
