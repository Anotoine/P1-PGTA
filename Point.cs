using System;

namespace P1_PGTA
{
    class Point
    {
        //Cartesians
        private double x;
        private double y;
        private double z;

        //WGS-84 - Degrees
        private double latD;
        private double lonD;
        private double altD;

        //WGS-84 - Radians
        private double latR;
        private double lonR;
        private double altR;

        //Polars
        private double rad;
        private double theta;
        private double phi;

        //ref lat lon for Lamber projection -> AIP airport
        private double latRef = 41.296531 * Math.PI / 180;
        private double lonRef = 2.075594 * Math.PI / 180;

        //standard latitudes 
        private double lat1 = 40 * Math.PI / 180;
        private double lat2 = 42 * Math.PI / 180;

        public Point(double lat, double lon) //
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
            this.x = rho * Math.Sin(n * (this.lonR - lonRef));
            this.y = rho0 - rho * Math.Cos(n * (this.lonR - lonRef));
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }
        public double LatR { get => latR; set => latR = value; }
        public double LonR { get => lonR; set => lonR = value; }
        public double AltR { get => altR; set => altR = value; }
        public double LatD { get => latD; set => latD = value; }
        public double LonD { get => lonD; set => lonD = value; }
        public double AltD { get => altD; set => altD = value; }
        public double Rad { get => rad; set => rad = value; }
        public double Theta { get => theta; set => theta = value; }
        public double Phi { get => phi; set => phi = value; }
    }
}
