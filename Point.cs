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

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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
