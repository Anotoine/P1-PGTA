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
using Microsoft.Win32;

namespace P1_PGTA
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


        private void Button_map_Click(object sender, RoutedEventArgs e)
        {
            float x = 41.29049F;
            float y = 2.098495F;
            Line l;
            string[] lines = File.ReadAllLines(@"BCN_Pistas.map");
            List<Point> points = new List<Point>();
            foreach (string line in lines)
            {
                string[] el = line.Split();
                if (el[0] == "Linea")
                {
                    for (int i = 1; i < 4; i += 2)
                    {
                        float a1 = Convert.ToSingle(el[i].Substring(0, 2)); // 41 grados
                        float b1 = Convert.ToSingle(el[i].Substring(2, 2)); // 17 minutos
                        float c1 = Convert.ToSingle(el[i].Substring(4, 2)); // 25 segundos
                        float d1 = Convert.ToSingle(el[i].Substring(6, 3)); // 761 milisegundos

                        float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                        float a2 = Convert.ToSingle(el[i+1].Substring(0, 3)); // 41 grados
                        float b2 = Convert.ToSingle(el[i+1].Substring(3, 2)); // 17 minutos
                        float c2 = Convert.ToSingle(el[i+1].Substring(5, 2)); // 25 segundos
                        float d2 = Convert.ToSingle(el[i+1].Substring(7, 3)); // 761 milisegundos

                        float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                        points.Add(new Point(x1, x2));
                    }

                    l = new Line();
                    l.Stroke = System.Windows.Media.Brushes.White;
                    l.StrokeThickness = 1;

                    Point zero0 = new Point(41.318936, 2.028951); //x y superior esquerra ?
                    double x0 = zero0.X;
                    double y0 = zero0.Y;
                    double A = -x0;
                    double B = -y0;
                    double alpha = A / 500;
                    double betha = B / 250;
                    Point a = points[points.Count - 1];
                    l.X1 = (a.X + A) / alpha;
                    l.Y1 = (a.Y + B) / betha;

                    Point b = points[points.Count - 2];
                    l.X2 = (b.X+A)/alpha;
                    l.Y2 = (b.Y+B)/betha;
                    Lienzo.Children.Add(l);
                }
            }
        }
    }
}
