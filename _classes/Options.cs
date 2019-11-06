using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ASTERIX
{
    class Options
    {
        string lang = "en-US";
        double interval = 1;

        public double Interval { get => interval; set => interval = value; }

        //Colors for the Canvas
        mapsColorClass mapsColor = new mapsColorClass();
        SolidColorBrush aircraftColor = Brushes.Red;
        SolidColorBrush vehiclesColor = Brushes.White;
        SolidColorBrush otherColor = Brushes.LightSkyBlue;

        public SolidColorBrush AircraftColor { get => aircraftColor; set => aircraftColor = value; }
        public SolidColorBrush VehiclesColor { get => vehiclesColor; set => vehiclesColor = value; }
        public SolidColorBrush OtherColor { get => otherColor; set => otherColor = value; }
        public mapsColorClass MapsColor { get => mapsColor; set => mapsColor = value; }


        public class mapsColorClass
        {
            SolidColorBrush backgroundColor = Brushes.Black;
            SolidColorBrush mainColor = Brushes.ForestGreen;
            SolidColorBrush secondaryColor = Brushes.Yellow;
            SolidColorBrush highlightColor = Brushes.LightSkyBlue;


            public SolidColorBrush BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
            public SolidColorBrush MainColor { get => mainColor; set => mainColor = value; }
            public SolidColorBrush SecondaryColor { get => secondaryColor; set => secondaryColor = value; }
            public SolidColorBrush HighlightColor { get => highlightColor; set => highlightColor = value; }
        }
    }
}
