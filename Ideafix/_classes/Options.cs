using System.Windows.Media;

namespace Ideafix
{
    public class Options
    {
        public string lang = "en-US";
        double interval = 1;

        public double Interval { get => interval; set => interval = value; }

        //Points Colors
        public SolidColorBrush AircraftColor { get; set; }
        public SolidColorBrush VehiclesColor { get; set; }
        public SolidColorBrush OtherColor { get; set; }

        //MapsColors
        public SolidColorBrush MapBackgroundColor { get; set; }
        public SolidColorBrush MapMainColor { get; set; }
        public SolidColorBrush MapSecondaryColor { get; set; }
        public SolidColorBrush MapHighlightColor { get; set; }
        public SolidColorBrush MapTextColor { get; set; }

        //Table Colors
        public SolidColorBrush TableMainColor { get; set; }
        public SolidColorBrush TableSecondaryColor { get; set; }
        public SolidColorBrush TableTextColor { get; set; }

        //Window Colors
        public SolidColorBrush WindowMainColor { get; set; }
        public SolidColorBrush WindowTextColor { get; set; }
        public SolidColorBrush WindowBackgroundColor { get; set; }

        //Time for Timer
        public string ActualTime { get; set; }

        public Options()
        {
            AircraftColor = Brushes.Red;
            VehiclesColor = Brushes.White;
            OtherColor = Brushes.MediumSpringGreen;

            MapBackgroundColor = Brushes.SlateGray;
            MapMainColor = Brushes.Tan;
            MapSecondaryColor = Brushes.LightYellow;
            MapHighlightColor = Brushes.Yellow;
            MapTextColor = Brushes.Black;

            TableMainColor = Brushes.LightSteelBlue;
            TableSecondaryColor = Brushes.LightGray;
            TableTextColor = Brushes.LightGray;

            WindowMainColor = Brushes.PaleVioletRed;
            WindowTextColor = Brushes.Black;
            WindowBackgroundColor = Brushes.DarkGray;

            ActualTime = "22:00:00";
        }
    }
}
