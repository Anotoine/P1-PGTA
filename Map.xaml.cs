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
            foreach (string line in lines)
            {
                string[] el = line.Split();
                if (el[0] == "Linea")
                {
                    float a = Convert.ToSingle(el[1].Substring(0, 2)); // 41 grados
                    float b = Convert.ToSingle(el[1].Substring(2, 2)); // 17 minutos
                    float c = Convert.ToSingle(el[1].Substring(4, 2)); // 25 segundos
                    float d = Convert.ToSingle(el[1].Substring(6, 3)); // 761 milisegundos
                    string ef = el[1].Substring(7); // N

                    float gg = Convert.ToSingle(c+d/1000);

                    float x1 = a + (b / 60) + (gg / 3600);

                    float a2 = Convert.ToSingle(el[2].Substring(0, 2)); // 41 grados
                    float b2 = Convert.ToSingle(el[2].Substring(2, 2)); // 17 minutos
                    float c2 = Convert.ToSingle(el[2].Substring(4, 2)); // 25 segundos
                    float d2 = Convert.ToSingle(el[2].Substring(6, 3)); // 761 milisegundos
                    //int ef2 = Convert.ToInt32(el[2].Substring(7)); // N

                    float gg2 = Convert.ToSingle(c2 + d2 / 1000);

                    float y1 = a2 + b2 / 60 + gg2 / 3600;


                    float a3 = Convert.ToSingle(el[3].Substring(0, 2)); // 41 grados
                    float b3 = Convert.ToSingle(el[3].Substring(2, 2)); // 17 minutos
                    float c3 = Convert.ToSingle(el[3].Substring(4, 2)); // 25 segundos
                    float d3 = Convert.ToSingle(el[3].Substring(6, 3)); // 761 milisegundos
                    //int ef3 = Convert.ToInt32(el[3].Substring(7)); // N

                    float gg3 = Convert.ToSingle(c3 + d3 / 1000);

                    float x2 = a3 + b3 / 60 + gg3 / 3600;


                    float a4 = Convert.ToSingle(el[4].Substring(0, 2)); // 41 grados
                    float b4 = Convert.ToSingle(el[4].Substring(2, 2)); // 17 minutos
                    float c4 = Convert.ToSingle(el[4].Substring(4, 2)); // 25 segundos
                    float d4 = Convert.ToSingle(el[4].Substring(6, 3)); // 761 milisegundos
                    //int ef4 = Convert.ToInt32(el[4].Substring(7)); // N

                    float gg4 = Convert.ToSingle(c4 + d4 / 1000);

                    float y2 = a4 + b4 / 60 + gg4 / 3600;


                    l = new Line();
                    l.Stroke = System.Windows.Media.Brushes.White;
                    l.StrokeThickness = 100;
                    l.X1 = 200*(x1 / x) + 500;
                    l.Y1 = 200*(y1 / y) + 250;

                    l.X2 = 200*(x2 / x) + 500;
                    l.Y2 = 200*(y2 / y) + 250;
                    Lienzo.Children.Add(l);


                }
            }
        }
    }
}
