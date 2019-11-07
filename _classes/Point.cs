﻿using System;

namespace ASTERIX
{
    public class Point
    {
        //Cartesians
        internal double X { get; set; }
        internal double Y { get; set; }
        internal double Z { get; set; }

        //WGS-84 - Degrees
        internal double latD { get; set; }
        internal double lonD { get; set; }
        internal double altD { get; set; }

        //WGS-84 - Radians
        internal double latR { get; set; }
        internal double lonR { get; set; }
        internal double altR { get; set; }

        //Polars
        internal double rad { get; set; }
        internal double theta { get; set; }
        internal double phi { get; set; }

        //ref lat lon for Lamber projection -> referenciat al centre de l'aeroport
        //private double latRef = 41.296531 * Math.PI / 180;
        //private double lonRef = 2.075594 * Math.PI / 180;
        private double latRef = 41.295885 * Math.PI / 180;
        private double lonRef = 2.086214 * Math.PI / 180;

        //standard latitudes 
        private double lat1 = 40 * Math.PI / 180;
        private double lat2 = 42 * Math.PI / 180;

        private double Re = 6378137;
        private double Rp = 6357000;
        private double Rm = 6371000;

        public Point()
        {

        }
        public Point LatLong2XY(double lat, double lon)
        {
            this.latD = lat;
            this.lonD = lon;
            this.latR = lat * Math.PI / 180;
            this.lonR = lon * Math.PI / 180;
            //Lambert conformal conic projection
            double num = Math.Log(Math.Cos(lat1) / Math.Cos(lat2)) / Math.Log(Math.Exp(1));
            double denum = Math.Log((Math.Tan(Math.PI / 4 + lat2 / 2)) / (Math.Tan(Math.PI / 4 + lat1 / 2))) / Math.Log(Math.Exp(1));
            double n = num / denum;
            double F = (Math.Cos(lat1) * Math.Pow(Math.Tan(Math.PI / 4 + lat1 / 2), n)) / n;
            double rho = F * Math.Pow(Math.Tan(Math.PI / 4 + this.latR / 2), -n);
            double rho0 = F * Math.Pow(Math.Tan(Math.PI / 4 + latRef / 2), -n);
            this.X = 1E6 * (rho * Math.Sin(n * (this.lonR - lonRef)));
            this.Y = 1E6 * (rho0 - rho * Math.Cos(n * (this.lonR - lonRef)));

            return this;
        }
        public Point XY2LatLong(double x, double y)
        {
            this.X = x;
            this.Y = y;
            //Lambert conformal conic reprojection
            //this.latR;

            return this;
        }
        public double[] xEyN(double latref, double lonref) {//retorna la distancia x (direccio E) i y (direcció N) entre el punt i el punt de referencia
            double lat = this.latR;
            double lon = this.lonR;
            double zarp = Rm * Math.Sin(latref);
            double z = Rm * Math.Sin(lat);
            double Rarp = Math.Sqrt((1 - Math.Pow(zarp, 2) / Math.Pow(Rp, 2)) * Math.Pow(Re, 2) + Math.Pow(zarp, 2));
            double R = Math.Sqrt((1 - Math.Pow(z, 2) / Math.Pow(Rp, 2)) * Math.Pow(Re, 2) + Math.Pow(z, 2));
            double Rlon = Rarp * Math.Cos(latref);
            double x = (lon - lonref) * Rlon;
            double y = (lat - latref) * R;
            return new double[] {x,y};
        }
    }
}
