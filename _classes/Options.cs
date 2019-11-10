using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ASTERIX
{
    public class Options
    {
        public string lang = "en-US";
        double interval = 1;

        public double Interval { get => interval; set => interval = value; }

        //Points Colors
        public SolidColorBrush AircraftColor { get ; set ; }
        public SolidColorBrush VehiclesColor { get ; set ; }
        public SolidColorBrush OtherColor { get ; set ; }

        //MapsColors
        public SolidColorBrush MapBackgroundColor { get ; set ; }
        public SolidColorBrush MapMainColor { get ; set ; }
        public SolidColorBrush MapSecondaryColor { get ; set ; }
        public SolidColorBrush MapHighlightColor { get ; set ; }

        //Table Colors
        public SolidColorBrush TableMainColor { get; set; }
        public SolidColorBrush TableSecondaryColor { get; set; }
        public SolidColorBrush TableTextColor { get; set; }

        //Window Colors
        public SolidColorBrush WindowMainColor { get; set; }
        public SolidColorBrush WindowTextColor { get; set; }
        public SolidColorBrush WindowBackgroundColor { get; set; }

        public Options()
        {
            AircraftColor = Brushes.Red;
            VehiclesColor = Brushes.White;
            OtherColor = Brushes.LightSkyBlue;

            MapBackgroundColor = Brushes.Black;
            MapMainColor = Brushes.ForestGreen;
            MapSecondaryColor = Brushes.LightYellow;
            MapHighlightColor = Brushes.Yellow;

            TableMainColor = Brushes.LightSteelBlue;
            TableSecondaryColor = Brushes.LightGray;
            TableTextColor = Brushes.LightGray;

            WindowMainColor = Brushes.PaleVioletRed;
            WindowTextColor = Brushes.White;
            WindowBackgroundColor = Brushes.DarkGray;
        }
    }
}
