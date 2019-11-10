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

        //ref lat lon -> centre de l'aeroport
        private double latRef = 41.296944 * Math.PI / 180;
        private double lonRef = 2.078333 * Math.PI / 180;

        private double latARP = 41.296944 * Math.PI / 180;
        private double lonARP = 2.078333 * Math.PI / 180;

        private double escalaX = 6.360;
        private double escalaY = 6.502;
        //private double latRef = 41.295885 * Math.PI / 180;
        //private double lonRef = 2.086214 * Math.PI / 180;

        //standard latitudes 
        //private double lat1 = 40 * Math.PI / 180;
        //private double lat2 = 42 * Math.PI / 180;
        private double lat1 = 43 * Math.PI / 180;
        private double lat2 = 36 * Math.PI / 180;



        private double Re = 6378137;
        private double Rp = 6357000;
        private double Rm = 6371000;

        public Point()
        {

        }
        public Point LatLong2XY(double lat, double lon) //centrat a ARP !!!
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
            double rho0 = F * Math.Pow(Math.Tan(Math.PI / 4 + latARP / 2), -n);
            this.X = 6.360*1E6 * (rho * Math.Sin(n * (this.lonR - lonARP)));
            this.Y = 6.502*1E6 * (rho0 - rho * Math.Cos(n * (this.lonR - lonARP)));

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
        public Point Polar2XY(double rho, double theta, double incX, double incY)
        {
            this.rad = rho;
            this.theta = theta;
            this.X = this.rad * Math.Sin(theta * Math.PI / 180) + incX;
            this.Y = this.rad * Math.Cos(theta * Math.PI / 180) + incY;

            return this;
        }
    }
}
