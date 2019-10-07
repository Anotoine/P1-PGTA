using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Asterix
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        public Map()
        {
            InitializeComponent();
        }

        Point zero0, ARP;
        double A, B, alpha, beta;
        List<List<Line>> mapsLines;
        List<List<Polyline>> mapsPolylines;
        List<CheckBox> checkBoxes;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxes = new List<CheckBox>();
            mapsLines = new List<List<Line>>();
            mapsPolylines = new List<List<Polyline>>();
            zero0 = new Point(41.315300, 2.043297); //x y superior esquerra ?
            ARP = new Point(41.296944, 2.078333); //ARP BCN airport --> Item 0

            A = -zero0.X;
            B = -zero0.Y;
            alpha = A / (Lienzo.ActualWidth / 2);
            beta = B / (Lienzo.ActualHeight / 2);
        }

        public void Load(object sender, RoutedEventArgs e)
        {
            string[] listfiles = Directory.GetFiles(@"maps/");

            foreach (string file in listfiles)
            {
                string[] lines = File.ReadAllLines(file);
                List<Line> mapL = new List<Line>();
                List<Polyline> mapP = new List<Polyline>();

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

                            tPoint.Add(new Point(x1, x2));
                        }
                        Line l = new Line();
                        l.StrokeThickness = 1;
                        l.X1 = (tPoint[0].X + A) / alpha;
                        l.Y1 = (tPoint[0].Y + B) / beta;

                        l.X2 = (tPoint[1].X + A) / alpha;
                        l.Y2 = (tPoint[1].Y + B) / beta;

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

                            tPoint.Add(new Point(x1, x2));
                            pp.Add(new System.Windows.Point((tPoint[tPoint.Count - 1].X + A) / alpha, (tPoint[tPoint.Count - 1].Y + B) / beta));
                        }
                        Polyline poly = new Polyline();
                        poly.Points = pp;
                        poly.StrokeThickness = 1;
                        mapP.Add(poly);
                        j += num;
                    }
                    else
                        j++;
                }
                mapsLines.Add(mapL);
                mapsPolylines.Add(mapP);
            }
        }

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            Lienzo.Children.Clear();
            if (CheckBCN.IsChecked == true)
            {
                foreach (Line l in mapsLines[0])
                {
                    l.Stroke = Brushes.Red;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[0])
                {
                    pl.Stroke = Brushes.Red;
                    Lienzo.Children.Add(pl);
                }
            }
            if (CheckCarretera.IsChecked == true)
            {
                foreach (Line l in mapsLines[1])
                {
                    l.Stroke = Brushes.White;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[1])
                {
                    pl.Stroke = Brushes.White;
                    Lienzo.Children.Add(pl);
                }
            }
            if (CheckMovimiento.IsChecked == true)
            {
                foreach (Line l in mapsLines[2])
                {
                    l.Stroke = Brushes.AliceBlue;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[2])
                {
                    pl.Stroke = Brushes.AliceBlue;
                    Lienzo.Children.Add(pl);
                }
            }
            if (CheckParking.IsChecked == true)
            {
                foreach (Line l in mapsLines[3])
                {
                    l.Stroke = Brushes.GreenYellow;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[3])
                {
                    pl.Stroke = Brushes.GreenYellow;
                    Lienzo.Children.Add(pl);
                }
            }
            if (CheckParterre.IsChecked == true)
            {
                foreach (Line l in mapsLines[4])
                {
                    l.Stroke = Brushes.Pink;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[4])
                {
                    pl.Stroke = Brushes.Pink;
                    Lienzo.Children.Add(pl);
                }
            }
            if (CheckPEdificios.IsChecked == true)
            {
                foreach (Line l in mapsLines[5])
                {
                    l.Stroke = Brushes.Orange;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[5])
                {
                    pl.Stroke = Brushes.Orange;
                    Lienzo.Children.Add(pl);
                }
            }
            if (CheckPistas.IsChecked == true)
            {
                foreach (Line l in mapsLines[6])
                {
                    l.Stroke = Brushes.LightGray;
                    Lienzo.Children.Add(l);
                }
                foreach (Polyline pl in mapsPolylines[6])
                {
                    pl.Stroke = Brushes.LightGray;
                    Lienzo.Children.Add(pl);
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

        }
    }
}