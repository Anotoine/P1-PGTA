﻿using System;
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

        Line l;
        Point zero0;
        double A, B, alpha, beta;
        List<List<Point>> maps;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            maps = new List<List<Point>>();
            zero0 = new Point(41.315300, 2.043297); //x y superior esquerra ?

            A = -zero0.X;
            B = -zero0.Y;
            alpha = A / (Lienzo.ActualWidth / 2);
            beta = B / (Lienzo.ActualHeight / 2);
        }


        public void Load(object sender, RoutedEventArgs e)
        {
            Polyline p;

            string[] listfiles = Directory.GetFiles(@"maps/");
            

            foreach (string file in listfiles)
            {
                string[] lines = File.ReadAllLines(file);
                List<Point> map = new List<Point>();

                map.Add(new Point(41.296531, 2.075594)); //ARP BCN airport --> Item 0

                int j = 0;
                while (j < lines.Length)
                {
                    string[] l1 = lines[j].Split();
                    if (l1[0] == "Linea")
                    {
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

                            map.Add(new Point(x1, x2));
                        }
                        j++;
                    }
                    else if (l1[0].StartsWith("Polilinea"))
                    {
                        int num = Convert.ToInt32(l1[1]);
                        PointCollection pp = new PointCollection();

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

                            //map.Add(new Point(x1, x2));
                            //pp.Add(new System.Windows.Point((map[map.Count - 1].X + A) / alpha, (map[map.Count - 1].Y + B) / beta));
                        }

                        p = new Polyline();
                        p.Stroke = Brushes.LightGreen;
                        p.StrokeThickness = 1;
                        //p.Points = pp;
                        Lienzo.Children.Add(p);
                        j += num;
                    }
                    else
                        j++;
                }
                maps.Add(map);
            }
        }

        private void CheckPistas_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < maps[0].Count; i+=2)
            {
                l = new Line();
                l.Stroke = Brushes.White;
                l.StrokeThickness = 1;

                Point a = maps[0][i];
                l.X1 = (a.X + A) / alpha;
                l.Y1 = (a.Y + B) / beta;

                Point b = maps[0][i + 1];
                l.X2 = (b.X + A) / alpha;
                l.Y2 = (b.Y + B) / beta;

                Lienzo.Children.Add(l);
            }
        }
    }
}




////                        l = new Line();
//l.Stroke = Brushes.White;
//                        l.StrokeThickness = 1;

//                        Point a = map[map.Count - 1];
//l.X1 = (a.X + A) / alpha;
//                        l.Y1 = (a.Y + B) / beta;

//                        Point b = map[map.Count - 2];
//l.X2 = (b.X + A) / alpha;
//                        l.Y2 = (b.Y + B) / beta;

//                        Lienzo.Children.Add(l);


//Ellipse ARP = new Ellipse();
//ARP.Stroke = Brushes.Red;
//                ARP.Fill = Brushes.Red;
//                ARP.Width = 5;
//                ARP.Height = 5;
//                Lienzo.Children.Add(ARP);
//                Canvas.SetLeft(ARP, ((map[0].X + A) / alpha) - ARP.Width / 2);
//                Canvas.SetTop(ARP, ((map[0].Y + B) / beta) - ARP.Height / 2);