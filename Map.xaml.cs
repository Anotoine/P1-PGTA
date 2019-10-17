using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ASTERIX
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {

        Point ARP, zero0;
        double A, B, AARP, BARP, alpha, beta, alphaARP, betaARP, propW, propH;
        List<List<Line>> mapsLines;
        List<List<Polyline>> mapsPolylines;
        List<Vehicle> VehiclesList;

        List<Message> listMessages;
        List<string> Vistos = new List<string>();

        List<CheckBox> checkBoxes;

        public Map(List<Message> messages)
        {
            InitializeComponent();
            checkBoxes = new List<CheckBox>();
            mapsLines = new List<List<Line>>();
            mapsPolylines = new List<List<Polyline>>();
            this.listMessages = messages;
            CreateAircrafts();
        }

        private void Butt_Refresh_Click(object sender, RoutedEventArgs e)
        {
            st1.Children.Clear();
            Lienzo.Children.Clear();
            Load(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            zero0 = new Point().LatLong2XY(41.315300, 2.043297); //x y superior esquerra 
            ARP = new Point().LatLong2XY(41.296944, 2.078333); //ARP BCN airport --> Item 0

            //xyz de Lambert --> xyz lienzo (amb origen de coordenades a d'alt a l'esquerra)
            A = -zero0.X;
            B = -zero0.Y;
            alpha = A / (Lienzo.ActualWidth / 2);
            beta = B / (Lienzo.ActualHeight / 2);

            //x y del ARP dins el lienzo en el size inicial
            double xarp_li = (ARP.X + A) / alpha;
            double yarp_li = (ARP.Y + B) / beta;
            propW = xarp_li / Lienzo.ActualWidth; //la proporció s'ha de mantenir!!!
            propH = yarp_li / Lienzo.ActualHeight;
            AARP = -Lienzo.ActualWidth * propW;
            BARP = -Lienzo.ActualHeight * propH;
            alphaARP = AARP / -2887;
            betaARP = BARP / 2078;
        }
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {   //entra just despres de fer el load
            alpha = A / (Lienzo.ActualWidth / 2);
            beta = B / (Lienzo.ActualHeight / 2);
            AARP = -Lienzo.ActualWidth * propW;
            BARP = -Lienzo.ActualHeight * propH;
            alphaARP = AARP / -2887;
            betaARP = BARP / 2078;
            CheckBoxClick(sender, e);
        }
        private void Lienzo_MouseMove(object sender, MouseEventArgs e)
        {
            PosXLabel.Text = ((e.GetPosition(Lienzo).X + AARP) / alphaARP).ToString("0.###m");
            PosYLabel.Text = ((e.GetPosition(Lienzo).Y + BARP) / betaARP).ToString("0.###m");
        }
        public void Load(object sender, RoutedEventArgs e)
        {
            checkBoxes = new List<CheckBox>();
            mapsLines = new List<List<Line>>();
            mapsPolylines = new List<List<Polyline>>();

            string path = Directory.GetCurrentDirectory();
            if (Directory.Exists(path + @"\maps\"))
            {
                string[] listfiles = Directory.GetFiles(path + @"\maps\");

                foreach (string file in listfiles)
                {
                    try
                    {
                        //Reading lines and creating Lists for Line and Polyline
                        string[] lines = File.ReadAllLines(file);
                        List<Line> mapL = new List<Line>();
                        List<Polyline> mapP = new List<Polyline>();

                        //Start to read the lines
                        int j = 0;
                        while (j < lines.Length)
                        {
                            string[] l1 = lines[j].Split();
                            if (l1[0].StartsWith("Linea"))
                            {
                                List<Point> tPoint = new List<Point>();
                                for (int i = 1; i < 4; i += 2)
                                {
                                    float a1 = Convert.ToSingle(l1[i].Substring(0, 2)); // grados
                                    float b1 = Convert.ToSingle(l1[i].Substring(2, 2)); // minutos
                                    float c1 = Convert.ToSingle(l1[i].Substring(4, 2)); // segundos
                                    float d1 = Convert.ToSingle(l1[i].Substring(6, 3)); // milisegundos

                                    float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                                    float a2 = Convert.ToSingle(l1[i + 1].Substring(0, 3)); // grados
                                    float b2 = Convert.ToSingle(l1[i + 1].Substring(3, 2)); // minutos
                                    float c2 = Convert.ToSingle(l1[i + 1].Substring(5, 2)); // segundos
                                    float d2 = Convert.ToSingle(l1[i + 1].Substring(7, 3)); // milisegundos

                                    float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                                    tPoint.Add(new Point().LatLong2XY(x1, x2));
                                }
                                Line l = new Line();

                                l.X1 = tPoint[0].X;
                                l.Y1 = tPoint[0].Y;

                                l.X2 = tPoint[1].X;
                                l.Y2 = tPoint[1].Y;

                                mapL.Add(l);
                                j++;
                            }
                            else if (l1[0].StartsWith("Polilinea"))
                            {
                                int num = Convert.ToInt32(l1[1]);
                                PointCollection pp = new PointCollection();
                                List<Point> tPoint = new List<Point>();

                                for (int i = j; i < j + num; i++)
                                {

                                    string[] l2 = lines[i + 1].Split();

                                    float a1 = Convert.ToSingle(l2[0].Substring(0, 2)); // grados
                                    float b1 = Convert.ToSingle(l2[0].Substring(2, 2)); // minutos
                                    float c1 = Convert.ToSingle(l2[0].Substring(4, 2)); // segundos
                                    float d1 = Convert.ToSingle(l2[0].Substring(6, 3)); // milisegundos

                                    float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                                    float a2 = Convert.ToSingle(l2[1].Substring(0, 3)); // grados
                                    float b2 = Convert.ToSingle(l2[1].Substring(3, 2)); // minutos
                                    float c2 = Convert.ToSingle(l2[1].Substring(5, 2)); // segundos
                                    float d2 = Convert.ToSingle(l2[1].Substring(7, 3)); // milisegundos

                                    float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                                    tPoint.Add(new Point().LatLong2XY(x1, x2));
                                    pp.Add(new System.Windows.Point(tPoint[tPoint.Count - 1].X, tPoint[tPoint.Count - 1].Y));
                                }
                                Polyline poly = new Polyline();
                                poly.Points = pp;

                                mapP.Add(poly);
                                j += num;
                            }
                            else
                                j++;
                        }
                        mapsLines.Add(mapL);
                        mapsPolylines.Add(mapP);

                        if (mapL.Count != 0)
                        {
                            //Creating the Checkbox to be checked
                            CheckBox checkBox = new CheckBox();
                            checkBox.Content = file.Substring(5);
                            checkBox.FontSize = 12;
                            checkBox.Foreground = Brushes.White;
                            checkBox.Margin = new Thickness(10, 10, 10, 10);
                            checkBox.Click += new RoutedEventHandler(CheckBoxClick);
                            checkBoxes.Add(checkBox);
                            st1.Children.Add(checkBox);
                        }
                    }
                    catch
                    {
                        //TODO: MessageBox saying the ones that could not be solved
                        MessageBox.Show("The file at: " + file + "could not be read. The file will be skipped.", "Error while reading.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("No maps folder found.\nDo you want to create the folder?\n (" + path + "/maps/)", "No directory found", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(path + "/maps/"); 
                }
            }
        }
        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            string str = "";
            Lienzo.Children.Clear();
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    foreach (Line l in mapsLines[i])
                    {
                        Line l1 = new Line();
                        l1.StrokeThickness = 1;
                        l1.Stroke = Brushes.ForestGreen;
                        l1.X1 = (l.X1 + A) / alpha;
                        l1.Y1 = (l.Y1 + B) / beta;

                        l1.X2 = (l.X2 + A) / alpha;
                        l1.Y2 = (l.Y2 + B) / beta;
                        Lienzo.Children.Add(l1);
                    }
                    foreach (Polyline pl in mapsPolylines[i])
                    {
                        Polyline poly = new Polyline();
                        poly.StrokeThickness = 1;
                        poly.Stroke = Brushes.ForestGreen;
                        PointCollection points = new PointCollection();
                        foreach (System.Windows.Point pp in pl.Points)
                            points.Add(new System.Windows.Point((pp.X + A) / alpha, (pp.Y + B) / beta));
                        poly.Points = points;
                        Lienzo.Children.Add(poly);
                    }
                }
            }

            if (CheckARP.IsChecked == true)
            {
                Ellipse ARPpoint = new Ellipse();
                ARPpoint.Stroke = Brushes.Yellow;
                ARPpoint.Fill = Brushes.Yellow;
                ARPpoint.Width = 5;
                ARPpoint.Height = 5;
                Lienzo.Children.Add(ARPpoint);
                Canvas.SetLeft(ARPpoint, ((ARP.X + A) / alpha) - ARPpoint.Width / 2);
                Canvas.SetTop(ARPpoint, ((ARP.Y + B) / beta) - ARPpoint.Height / 2);
            }

            if (CheckVehicles.IsChecked == true)
            {
                foreach (Vehicle v in VehiclesList)
                {
                    str += v.TrackN + "-" + v.ICAOaddress + "-" + v.Callsign + "\n";
                    foreach (Point p in v.GetPointsByDate(new DateTime().AddHours(28)))
                    {
                        if (v.Type == "Aircraft")
                        {
                            Ellipse p0 = new Ellipse();

                            //if (v.Callsign.StartsWith("F"))
                            //    p0.Stroke = Brushes.White;
                            if (v.Callsign == "NONE")
                            {
                                p0.Stroke = Brushes.Red;
                                p0.StrokeThickness = 10;
                            }
                            else
                                p0.Stroke = Brushes.AliceBlue;

                            p0.StrokeThickness = 1;
                            p0.Width = 5;
                            p0.Height = p0.Width;
                            p0.Tag = v.TrackN;
                            p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                            Lienzo.Children.Add(p0);
                            
                            Canvas.SetLeft(p0, (p.X * alphaARP) - AARP - p0.Width / 2);
                            Canvas.SetTop(p0, (p.Y * betaARP) - BARP - p0.Height / 2);
                        }
                        else
                        {
                            Ellipse p0 = new Ellipse();
                            p0.Stroke = Brushes.Yellow;
                            p0.StrokeThickness = 1;
                            p0.Width = 1;
                            p0.Height = p0.Width;
                            Lienzo.Children.Add(p0);

                            Canvas.SetLeft(p0, p.X * alphaARP - AARP - p0.Width / 2);
                            Canvas.SetTop(p0, p.Y * betaARP - BARP - p0.Height / 2);
                        }
                    }
                }
            }
            MessageBox.Show(str);
        }
        private void CreateAircrafts()
        {
            if (!(listMessages == null))
            {
                VehiclesList = new List<Vehicle>();
                foreach (Message m in listMessages)
                {
                    if (Vistos.Contains(m.getAddressICAO()))
                    {
                        VehiclesList[Vistos.IndexOf(m.getAddressICAO())].AddPoint(m);
                    }
                    else
                    {
                        Vistos.Add(m.getAddressICAO());
                        VehiclesList.Add(new Vehicle(m));
                    }
                }
            }
        }
        private void PlaneClick(object sender, RoutedEventArgs e)
        {
            int trackN = Convert.ToInt32((e.OriginalSource as FrameworkElement).Tag.ToString());

            bool exit = false; int i = 0;
            while (!(exit || i >= VehiclesList.Count))
            {
                if (VehiclesList[i].TrackN == trackN)
                    exit = true;
                i++;
            }

            string str = "ICAO Addres: " + VehiclesList[i].ICAOaddress + "\n" + "Callsign: " + VehiclesList[i].Callsign + "\n"
                + "X: " + VehiclesList[i].Positions[0].X + "m\n" + "Y: " + VehiclesList[i].Positions[0].Y + "m\n"
                + "XCanvas: " + Convert.ToString((VehiclesList[i].Positions[0].X * alphaARP) -AARP) + "m\n" +
                "YCanvas: " + Convert.ToString((VehiclesList[i].Positions[0].Y * betaARP) - BARP) + "m\n";
            MessageBox.Show(str, "TN: " + VehiclesList[i].TrackN);
        }
    }
}