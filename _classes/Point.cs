using System;

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

        //ref lat lon for Lamber projection -> referenciat al centre de l'atirport
        //private double latRef = 41.296531 * Math.PI / 180;
        //private double lonRef = 2.075594 * Math.PI / 180;
        private double latRef = 41.295885 * Math.PI / 180;
        private double lonRef = 2.086214 * Math.PI / 180;


        //standard latitudes 
        private double lat1 = 40 * Math.PI / 180;
        private double lat2 = 42 * Math.PI / 180;

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

    }
}
