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
        string lang = "ENG";
        double fm = 1;

        //Colors for the Canvas
        SolidColorBrush mapsColor = Brushes.ForestGreen;
        SolidColorBrush aircraftColor = Brushes.Red;
        SolidColorBrush vehiclesColor = Brushes.White;
        SolidColorBrush otherColor = Brushes.LightSkyBlue;

        public SolidColorBrush MapsColor { get => mapsColor; set => mapsColor = value; }
        public SolidColorBrush AircraftColor { get => aircraftColor; set => aircraftColor = value; }
        public SolidColorBrush VehiclesColor { get => vehiclesColor; set => vehiclesColor = value; }
        public SolidColorBrush OtherColor { get => otherColor; set => otherColor = value; }
    }
}
