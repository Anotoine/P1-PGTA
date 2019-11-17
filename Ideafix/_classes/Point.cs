using System;

namespace Ideafix
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

        internal double[] DMSlat { get; set; }
        internal double[] DMSlon { get; set; }
        internal string DMSlatString { get; set; }
        internal string DMSlonString { get; set; }
        //WGS-84 - Radians
        internal double latR { get; set; }
        internal double lonR { get; set; }
        internal double altR { get; set; }

        //Polars
        internal double rad { get; set; }
        internal double theta { get; set; }
        internal double phi { get; set; }

        //ref lat lon -> centre de l'aeroport
        private readonly double latRef = 41.296944 * Math.PI / 180;
        private readonly double lonRef = 2.078333 * Math.PI / 180;

        private readonly double latARP = 41.296944 * Math.PI / 180;
        private readonly double lonARP = 2.078333 * Math.PI / 180;

        private readonly double escalaX = 6.360;
        private readonly double escalaY = 6.502;

        //standard latitudes 
        private readonly double lat1 = 43 * Math.PI / 180;
        private readonly double lat2 = 36 * Math.PI / 180;

        private readonly double Re = 6378137;
        private readonly double Rp = 6357000;
        private readonly double Rm = 6371000;

        public Point()
        {

        }

        public Point FromLatLong(double lat, double lon) //centrat a ARP !!!
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
            this.X = 6.360 * 1E6 * (rho * Math.Sin(n * (this.lonR - lonARP)));
            this.Y = 6.502 * 1E6 * (rho0 - rho * Math.Cos(n * (this.lonR - lonARP)));

            this.rad = Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
            this.theta = Math.Atan(this.Y / this.X);

            DMSAdd();
            return this;
        }

        public Point FromCartesian(double x, double y)
        {
            this.X = x;
            this.Y = y;
            x /= (6.360 * 1E6);
            y /= (6.502 * 1E6);
            double num = Math.Log(Math.Cos(lat1) / Math.Cos(lat2)) / Math.Log(Math.Exp(1));
            double denum = Math.Log((Math.Tan(Math.PI / 4 + lat2 / 2)) / (Math.Tan(Math.PI / 4 + lat1 / 2))) / Math.Log(Math.Exp(1));
            double n = num / denum;
            double F = (Math.Cos(lat1) * Math.Pow(Math.Tan(Math.PI / 4 + lat1 / 2), n)) / n;
            double rho0 = F * Math.Pow(Math.Tan(Math.PI / 4 + latARP / 2), -n);
            double rho = Math.Sign(n) * Math.Sqrt(Math.Pow(x, 2) + Math.Pow((rho0 - y), 2));
            double theta = Math.Pow(Math.Atan(x / (rho0 - y)), 1);
            this.latR = 2 * Math.Pow(Math.Atan(Math.Pow(F / rho, 1 / n)), 1) - Math.PI / 2;
            this.lonR = lonARP + theta / n;
            this.latD = this.latR * (180 / Math.PI);
            this.lonD = this.lonR * (180 / Math.PI);

            this.rad = Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
            this.theta = Math.Atan(this.Y / this.X);

            DMSAdd();
            return this;
        }

        public Point FromPolar(double r, double the, double incX, double incY)
        {
            this.rad = r;
            this.theta = the;
            this.X = this.rad * Math.Sin(the * Math.PI / 180) + incX;
            this.Y = this.rad * Math.Cos(the * Math.PI / 180) + incY;

            this.X /= (6.360 * 1E6);
            this.Y /= (6.502 * 1E6);
            double num = Math.Log(Math.Cos(lat1) / Math.Cos(lat2)) / Math.Log(Math.Exp(1));
            double denum = Math.Log((Math.Tan(Math.PI / 4 + lat2 / 2)) / (Math.Tan(Math.PI / 4 + lat1 / 2))) / Math.Log(Math.Exp(1));
            double n = num / denum;
            double F = (Math.Cos(lat1) * Math.Pow(Math.Tan(Math.PI / 4 + lat1 / 2), n)) / n;
            double rho0 = F * Math.Pow(Math.Tan(Math.PI / 4 + latARP / 2), -n);
            double rho = Math.Sign(n) * Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow((rho0 - this.Y), 2));
            double theta = Math.Pow(Math.Atan(this.X / (rho0 - this.Y)), 1);
            this.latR = 2 * Math.Pow(Math.Atan(Math.Pow(F / rho, 1 / n)), 1) - Math.PI / 2;
            this.lonR = lonARP + theta / n;
            this.latD = this.latR * (180 / Math.PI);
            this.lonD = this.lonR * (180 / Math.PI);

            DMSAdd();
            return this;
        }

        public void DMSAdd()
        {
            //DMS vector creation
            this.DMSlat = new double[3];
            this.DMSlon = new double[3];
            this.DMSlat[0] = Math.Floor(latD);
            this.DMSlat[1] = Math.Floor((latD - DMSlat[0]) * 60);
            this.DMSlat[2] = ((latD - DMSlat[0]) * 60 - DMSlat[1]) * 60;
            this.DMSlon[0] = Math.Floor(lonD);
            this.DMSlon[1] = Math.Floor((lonD - DMSlon[0]) * 60);
            this.DMSlon[2] = ((lonD - DMSlon[0]) * 60 - DMSlon[1]) * 60;

            this.DMSlatString = this.DMSlat[0] + "º" + this.DMSlat[1] + "'" + this.DMSlat[2].ToString("00.####") + '"';
            this.DMSlonString = this.DMSlon[0] + "º" + this.DMSlon[1] + "'" + this.DMSlon[2].ToString("00.####") + '"';
        }
    }
}
