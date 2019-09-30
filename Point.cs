using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P1_PGTA
{
    class Point
    {
        //Cartesians
        private double x;
        private double y;
        private double z;

        //WGS-84
        private double lat;
        private double lon;
        private double alt;

        //Polars
        private double rad;
        private double theta;
        private double phi;

        //ref lat lon for Lamber projection -> AIP airport
        private double lat0 = 41.296531 * Math.PI / 180;
        private double lon0 = 2.075594 * Math.PI / 180;
        //standard latitudes 
        private double lat1 = 40 * Math.PI / 180;
        private double lat2 = 48 * Math.PI / 180;

        public Point(double x1, double y1, bool IsXY)
        {
            if (IsXY)
            {
                this.x = x1;
                this.y = y1;
                updateLatLon();
            } else {
                this.lat = x1;
                this.lon = y1;
                updateXY();
            }
        }

        private void updateXY()
        {
            //Lambert conformal conic projection
            double num = Math.Log(Math.Cos(lat1) / Math.Cos(lat2)) / Math.Log(Math.Exp(1));
            double denum = Math.Log((Math.Tan(Math.PI / 4 + lat2 / 2)) / (Math.Tan(Math.PI / 4 + lat1 / 2))) / Math.Log(Math.Exp(1));
            double n = num / denum;
            double F = (Math.Cos(lat1) * Math.Pow(Math.Tan(Math.PI / 4 + lat1 / 2), n)) / n;
            double rho = F * Math.Pow(Math.Tan(Math.PI / 4 + lat / 2), -n);
            double rho0 = F * Math.Pow(Math.Tan(Math.PI / 4 + lat0 / 2), -n);
            this.x = rho * Math.Sin(n * (lon - lon0));
            this.y = rho0 - rho * Math.Cos(n * (lon - lon0));
        }

        private void updateLatLon()
        {

        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }
        public double Lat { get => lat; set => lat = value; }
        public double Lon { get => lon; set => lon = value; }
        public double Alt { get => alt; set => alt = value; }
        public double Rad { get => rad; set => rad = value; }
        public double Theta { get => theta; set => theta = value; }
        public double Phi { get => phi; set => phi = value; }
    }
}
