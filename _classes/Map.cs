using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTERIX
{
    class Map
    {
        List<Tuple<Point, Point>> Lines = new List<Tuple<Point, Point>>();
        List<Tuple<Point, int>> Simbolos = new List<Tuple<Point, int>>();
        List<Tuple<Point, string>> Textos = new List<Tuple<Point, string>>();
        List<List<Point>> Polylines = new List<List<Point>>();
        string Name;

        public Map(string file)
        {
            //Reading lines and creating Lists for Line and Polyline
            string[] lines = File.ReadAllLines(file);

            //Start to read the lines
            int j = 0;
            while (j < lines.Length)
            {
                if (!(lines[j].StartsWith("#") || string.IsNullOrEmpty(lines[j])))
                {
                    string[] l1 = lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (l1[0].StartsWith("Linea"))
                    {
                        List<Point> tPoint = new List<Point>();
                        for (int i = 1; i < 4; i += 2)
                        {
                            float a1 = Convert.ToSingle(l1[i].Substring(0, 2)); // grados
                            float b1 = Convert.ToSingle(l1[i].Substring(2, 2)); // minutos
                            float c1 = Convert.ToSingle(l1[i].Substring(4, 2)); // segundos
                            float d1 = 0;
                            if (!(l1[i].Substring(6, 1) == "N" || l1[i].Substring(6, 1) == "S"))
                                d1 = Convert.ToSingle(l1[i].Substring(6, 3)); // milisegundos -- if applicable

                            //Obtaining the final value
                            float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);
                            
                            //Checking if should be positive or negative
                            if (l1[i].EndsWith("S"))
                                x1 = -1 * x1;

                            float a2 = Convert.ToSingle(l1[i + 1].Substring(0, 3)); // grados
                            float b2 = Convert.ToSingle(l1[i + 1].Substring(3, 2)); // minutos
                            float c2 = Convert.ToSingle(l1[i + 1].Substring(5, 2)); // segundos
                            float d2 = 0;
                            if (!(l1[i + 1].Substring(7, 1) == "E" || l1[i + 1].Substring(7, 1) == "W" || l1[i + 1].Substring(7, 1) == "O"))
                                d2 = Convert.ToSingle(l1[i + 1].Substring(7, 3)); // milisegundos  -- if applicable

                            //Obtaining the final value
                            float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (l1[i + 1].EndsWith("O") || l1[i + 1].EndsWith("W"))
                                x2 = -1 * x2;

                            tPoint.Add(new Point().LatLong2XY(x1, x2));
                        }
                        Lines.Add(new Tuple<Point, Point>(tPoint[0], tPoint[1]));
                        j++;
                    }
                    else if (l1[0].StartsWith("Texto"))
                    {
                        float a1 = Convert.ToSingle(l1[1].Substring(0, 2)); // grados
                        float b1 = Convert.ToSingle(l1[1].Substring(2, 2)); // minutos
                        float c1 = Convert.ToSingle(l1[1].Substring(4, 2)); // segundos
                        float d1 = 0;
                        if (!(l1[1].Substring(6, 1) == "N" || l1[1].Substring(6, 1) == "S"))
                            d1 = Convert.ToSingle(l1[1].Substring(6, 3)); // milisegundos -- if applicable

                        //Obtaining the final value
                        float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                        //Checking if should be positive or negative
                        if (l1[1].EndsWith("S"))
                            x1 = -1 * x1;

                        float a2 = Convert.ToSingle(l1[2].Substring(0, 3)); // grados
                        float b2 = Convert.ToSingle(l1[2].Substring(3, 2)); // minutos
                        float c2 = Convert.ToSingle(l1[2].Substring(5, 2)); // segundos
                        float d2 = 0;
                        if (!(l1[2].Substring(7, 1) == "E" || l1[2].Substring(7, 1) == "W" || l1[2].Substring(7, 1) == "O"))
                            d2 = Convert.ToSingle(l1[2].Substring(7, 3)); // milisegundos  -- if applicable

                        //Obtaining the final value
                        float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                        //Checking if should be positive or negative
                        if (l1[2].EndsWith("O") || l1[2].EndsWith("W"))
                            x2 = -1 * x2;

                        Textos.Add(new Tuple<Point, string>(new Point().LatLong2XY(x1, x2), l1[3]));
                        j++;
                    }
                    else if (l1[0].StartsWith("Simbolo"))
                    {
                        float a1 = Convert.ToSingle(l1[1].Substring(0, 2)); // grados
                        float b1 = Convert.ToSingle(l1[1].Substring(2, 2)); // minutos
                        float c1 = Convert.ToSingle(l1[1].Substring(4, 2)); // segundos
                        float d1 = 0;
                        if (!(l1[1].Substring(6, 1) == "N" || l1[1].Substring(6, 1) == "S"))
                            d1 = Convert.ToSingle(l1[1].Substring(6, 3)); // milisegundos -- if applicable

                        //Obtaining the final value
                        float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                        //Checking if should be positive or negative
                        if (l1[1].EndsWith("S"))
                            x1 = -1 * x1;

                        float a2 = Convert.ToSingle(l1[2].Substring(0, 3)); // grados
                        float b2 = Convert.ToSingle(l1[2].Substring(3, 2)); // minutos
                        float c2 = Convert.ToSingle(l1[2].Substring(5, 2)); // segundos
                        float d2 = 0;
                        if (!(l1[2].Substring(7, 1) == "E" || l1[2].Substring(7, 1) == "W" || l1[2].Substring(7, 1) == "O"))
                            d2 = Convert.ToSingle(l1[2].Substring(7, 3)); // milisegundos  -- if applicable

                        //Obtaining the final value
                        float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                        //Checking if should be positive or negative
                        if (l1[2].EndsWith("O") || l1[2].EndsWith("W"))
                            x2 = -1 * x2;

                        Simbolos.Add(new Tuple<Point, int>(new Point().LatLong2XY(x1, x2), Convert.ToInt32(l1[3])));
                        j++;
                    }
                    else if (l1[0].StartsWith("Polilinea"))
                    {
                        int num = Convert.ToInt32(l1[1]);
                        List<Point> pp = new List<Point>();

                        for (int i = j; i < j + num; i++)
                        {
                            string[] l2 = lines[i + 1].Split();

                            float a1 = Convert.ToSingle(l2[0].Substring(0, 2)); // grados
                            float b1 = Convert.ToSingle(l2[0].Substring(2, 2)); // minutos
                            float c1 = Convert.ToSingle(l2[0].Substring(4, 2)); // segundos
                            float d1 = 0;
                            if (!(l2[0].Substring(6, 1) == "N" || l2[0].Substring(6, 1) == "S"))
                                d1 = Convert.ToSingle(l2[0].Substring(6, 3)); // milisegundos  -- if applicable

                            //Obtaining the final value
                            float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (l1[0].EndsWith("S"))
                                x1 = -1 * x1;

                            float a2 = Convert.ToSingle(l2[1].Substring(0, 3)); // grados
                            float b2 = Convert.ToSingle(l2[1].Substring(3, 2)); // minutos
                            float c2 = Convert.ToSingle(l2[1].Substring(5, 2)); // segundos
                            float d2 = 0;
                            if (!(l2[1].Substring(7, 1) == "E" || l2[1].Substring(7, 1) == "W" || l2[1].Substring(7, 1) == "O"))
                                d2 = Convert.ToSingle(l2[1].Substring(7, 3)); // milisegundos

                            //Obtaining the final value
                            float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (l2[1].EndsWith("O") || l2[1].EndsWith("W"))
                                x2 = -1 * x2;

                            pp.Add(new Point().LatLong2XY(x1, x2));
                        }
                        Polylines.Add(pp);
                        j += num + 1;
                    }
                    else
                        j++;
                }
                else
                    j++;
            }

            Name = file.Split('\\')[file.Split('\\').Length - 1];

            //e.Result = listfiles.ToString();
        }

        public string getName()
        {
            return this.Name;
        }
        
        public List<Tuple<Point, Point>> getPoints()
        {
            return this.Lines;
        }

        public List<List<Point>> getPolylines()
        {
            return this.Polylines;
        }

        public List<Tuple<Point, int>> getSimbols()
        {
            return this.Simbolos;
        }

        public List<Tuple<Point, string>> getTexts()
        {
            return this.Textos;
        }
    }
}
