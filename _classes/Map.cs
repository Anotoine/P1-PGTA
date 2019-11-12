using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ideafix
{
    class Map
    {
        string Name;
        
        List<Tuple<Point, Point>> Lines = new List<Tuple<Point, Point>>();
        List<Tuple<Point, string>> Simbols = new List<Tuple<Point, string>>();
        List<Tuple<Point, string>> Texts = new List<Tuple<Point, string>>();
        List<List<Point>> Polylines = new List<List<Point>>();
        List<List<Point>> Polygons = new List<List<Point>>();

        //Index for Polygon
        List<int> index = new List<int>();

        SolidColorBrush SimbolsColor = new SolidColorBrush();
        SolidColorBrush TextsColor = new SolidColorBrush();
        SolidColorBrush LinesColor = new SolidColorBrush();

        Dictionary<int, Color> ListColorsPoligons = new Dictionary<int, Color>();

        public Map(string file)
        {
            //Reading lines and creating Lists for Line and Polyline
            string[] lines = File.ReadAllLines(file);

            //Start to read the lines
            int j = 0;
            while (j < lines.Length)
            {
                string[] auxstr, auxstr2;
                float a, b, c, d, x, y;
                int lengthN, lengthW, num;
                List<Point> pp;
                if (!(string.IsNullOrEmpty(lines[j]) || string.IsNullOrWhiteSpace(lines[j])))
                {
                    switch (lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0])
                    {
                        case "TituloMapa":
                            Name = lines[j].Substring(10).Trim(new char[] { ' ', '\t' });
                            j++;
                            break;

                        case "ColorTexto":
                            auxstr = lines[j].Substring(10).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            TextsColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(auxstr[0], 10), Convert.ToByte(auxstr[1], 10), Convert.ToByte(auxstr[2], 10)));

                            j++;
                            break;

                        case "ColorSimbolos":
                            auxstr = lines[j].Substring(13).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            SimbolsColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(auxstr[0], 10), Convert.ToByte(auxstr[1], 10), Convert.ToByte(auxstr[2], 10)));

                            j++;
                            break;

                        case "ColorLinea":
                            auxstr = lines[j].Substring(10).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            LinesColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(auxstr[0], 10), Convert.ToByte(auxstr[1], 10), Convert.ToByte(auxstr[2], 10)));

                            j++;
                            break;

                        case "ColorPoligono":
                            auxstr = lines[j].Substring(13).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            int pos = Convert.ToInt16(auxstr[0]);
                            int R = Convert.ToInt16(auxstr[1]);
                            int G = Convert.ToInt16(auxstr[2]);
                            int B = Convert.ToInt16(auxstr[3]);
                            ListColorsPoligons.Add(pos, Color.FromArgb(100, (byte)R, (byte)G, (byte)B));


                            j++;
                            break;


                        case "Simbolo":
                            auxstr = lines[j].Substring(7).Trim(new char[] { ' ', '\t' }).Split('N');
                            lengthN = auxstr[0].Length;

                            a = Convert.ToSingle(auxstr[0].Substring(0, 2), null); // grados
                            b = Convert.ToSingle(auxstr[0].Substring(2, 2), null); // minutos
                            c = Convert.ToSingle(auxstr[0].Substring(4, 2), null); // segundos
                            d = 0;
                            if (lengthN > 6)
                                d = Convert.ToSingle(auxstr[0].Substring(6, 3), null); // milisegundos -- if applicable

                            //Obtaining the final value
                            x = a + (b / 60) + ((c + d / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (auxstr[0].EndsWith("S", StringComparison.Ordinal))
                                x = -1 * x;


                            auxstr = auxstr[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            lengthW = auxstr[0].Length;
                            a = Convert.ToSingle(auxstr[0].Substring(0, 3), null); // grados
                            b = Convert.ToSingle(auxstr[0].Substring(3, 2), null); // minutos
                            c = Convert.ToSingle(auxstr[0].Substring(5, 2), null); // segundos
                            d = 0;
                            if (lengthW > 8)
                                d = Convert.ToSingle(auxstr[0].Substring(7, 3), null); // milisegundos  -- if applicable

                            //Obtaining the final value
                            y = a + (b / 60) + ((c + d / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (auxstr[0].EndsWith("O", StringComparison.Ordinal) || auxstr[0].EndsWith("W", StringComparison.Ordinal))
                                y = -1 * y;

                            Simbols.Add(new Tuple<Point, string>(new Point().LatLong2XY(x, y), lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[2]));

                            j++;
                            break;

                        case "Texto":
                            auxstr = lines[j].Substring(7).Trim(new char[] { ' ', '\t' }).Split('N');
                            lengthN = auxstr[0].Length;

                            a = Convert.ToSingle(auxstr[0].Substring(0, 2), null); // grados
                            b = Convert.ToSingle(auxstr[0].Substring(2, 2), null); // minutos
                            c = Convert.ToSingle(auxstr[0].Substring(4, 2), null); // segundos
                            d = 0;
                            if (lengthN > 6)
                                d = Convert.ToSingle(auxstr[0].Substring(6, 3), null); // milisegundos -- if applicable

                            //Obtaining the final value
                            x = a + (b / 60) + ((c + d / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (auxstr[0].EndsWith("S", StringComparison.Ordinal))
                                x = -1 * x;


                            auxstr = auxstr[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            lengthW = auxstr[0].Length;
                            a = Convert.ToSingle(auxstr[0].Substring(0, 3), null); // grados
                            b = Convert.ToSingle(auxstr[0].Substring(3, 2), null); // minutos
                            c = Convert.ToSingle(auxstr[0].Substring(5, 2), null); // segundos
                            d = 0;
                            if (lengthW > 8)
                                d = Convert.ToSingle(auxstr[0].Substring(7, 3), null); // milisegundos  -- if applicable
                            
                            //Obtaining the final value
                            y = a + (b / 60) + ((c + d / 1000) / 3600);

                            //Checking if should be positive or negative
                            if (auxstr[0].EndsWith("O", StringComparison.Ordinal) || auxstr[0].EndsWith("W", StringComparison.Ordinal))
                                y = -1 * y;

                            Texts.Add(new Tuple<Point, string>(new Point().LatLong2XY(x, y), lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[2]));

                            j++;
                            break;

                        case "Linea":
                            MatchCollection match = Regex.Matches(lines[j].Substring(5).Trim(), @"\d{5,9}(N|S){1}\s*\d{5,11}(E|W|O){1}");
                            auxstr2 = new string[] { match[0].Value, match[1].Value };
                            List<Point> tPoint = new List<Point>();

                            for (int i = 0; i < 2; i ++)
                            {
                                auxstr = auxstr2[i].Trim(new char[] { ' ', '\t' }).Split('N');
                                lengthN = auxstr[0].Length;

                                a = Convert.ToSingle(auxstr[0].Substring(0, 2), null); // grados
                                b = Convert.ToSingle(auxstr[0].Substring(2, 2), null); // minutos
                                c = Convert.ToSingle(auxstr[0].Substring(4, 2), null); // segundos
                                d = 0;
                                if (lengthN > 6)
                                    d = Convert.ToSingle(auxstr[0].Substring(6, 3), null); // milisegundos -- if applicable

                                //Obtaining the final value
                                x = a + (b / 60) + ((c + d / 1000) / 3600);

                                //Checking if should be positive or negative
                                if (auxstr[0].EndsWith("S", StringComparison.Ordinal))
                                    x = -1 * x;


                                auxstr = auxstr[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                lengthW = auxstr[0].Length;
                                a = Convert.ToSingle(auxstr[0].Substring(0, 3), null); // grados
                                b = Convert.ToSingle(auxstr[0].Substring(3, 2), null); // minutos
                                c = Convert.ToSingle(auxstr[0].Substring(5, 2), null); // segundos
                                d = 0;
                                if (lengthW > 8)
                                    d = Convert.ToSingle(auxstr[0].Substring(7, 3), null); // milisegundos  -- if applicable


                                //Obtaining the final value
                                y = a + (b / 60) + ((c + d / 1000) / 3600);

                                //Checking if should be positive or negative
                                if (auxstr[0].EndsWith("O", StringComparison.Ordinal) || auxstr[0].EndsWith("W", StringComparison.Ordinal))
                                    y = -1 * y;

                                tPoint.Add(new Point().LatLong2XY(x, y));
                            }
                            Lines.Add(new Tuple<Point, Point>(tPoint[0], tPoint[1]));

                            j++;
                            break;

                        case "Polilinea":
                            num = Convert.ToInt32(lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1], 10);
                            pp = new List<Point>();

                            for (int i = j + 1; i <= j + num; i++)
                            {
                                auxstr = lines[i].Trim(new char[] { ' ', '\t' }).Split('N');
                                lengthN = auxstr[0].Length;

                                a = Convert.ToSingle(auxstr[0].Substring(0, 2), null); // grados
                                b = Convert.ToSingle(auxstr[0].Substring(2, 2), null); // minutos
                                c = Convert.ToSingle(auxstr[0].Substring(4, 2), null); // segundos
                                d = 0;
                                if (lengthN > 6)
                                    d = Convert.ToSingle(auxstr[0].Substring(6, 3), null); // milisegundos -- if applicable

                                //Obtaining the final value
                                x = a + (b / 60) + ((c + d / 1000) / 3600);

                                //Checking if should be positive or negative
                                if (auxstr[0].EndsWith("S", StringComparison.Ordinal))
                                    x = -1 * x;

                                auxstr = auxstr[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                lengthW = auxstr[0].Length;
                                a = Convert.ToSingle(auxstr[0].Substring(0, 3), null); // grados
                                b = Convert.ToSingle(auxstr[0].Substring(3, 2), null); // minutos
                                c = Convert.ToSingle(auxstr[0].Substring(5, 2), null); // segundos
                                d = 0;
                                if (lengthW > 8)
                                    d = Convert.ToSingle(auxstr[0].Substring(7, 3), null); // milisegundos  -- if applicable

                                //Obtaining the final value
                                y = a + (b / 60) + ((c + d / 1000) / 3600);

                                //Checking if should be positive or negative
                                if (auxstr[0].EndsWith("O", StringComparison.Ordinal) || auxstr[0].EndsWith("W", StringComparison.Ordinal))
                                    y = -1 * y;

                                pp.Add(new Point().LatLong2XY(x, y));
                            }
                            Polylines.Add(pp);

                            j += num;
                            break;

                        case "Poligono":
                            num = Convert.ToInt32(lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1], 10);
                            pp = new List<Point>();
                            index.Add(Convert.ToInt32(lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[2], 10));

                            for (int i = j + 1; i <= j + num; i++)
                            {
                                auxstr = lines[i].Trim(new char[] { ' ', '\t' }).Split('N');
                                lengthN = auxstr[0].Length;

                                a = Convert.ToSingle(auxstr[0].Substring(0, 2), null); // grados
                                b = Convert.ToSingle(auxstr[0].Substring(2, 2), null); // minutos
                                c = Convert.ToSingle(auxstr[0].Substring(4, 2), null); // segundos
                                d = 0;
                                if (lengthN > 6)
                                    d = Convert.ToSingle(auxstr[0].Substring(6, 3), null); // milisegundos -- if applicable

                                //Obtaining the final value
                                x = a + (b / 60) + ((c + d / 1000) / 3600);

                                //Checking if should be positive or negative
                                if (auxstr[0].EndsWith("S", StringComparison.Ordinal))
                                    x = -1 * x;

                                auxstr = auxstr[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                lengthW = auxstr[0].Length;
                                a = Convert.ToSingle(auxstr[0].Substring(0, 3), null); // grados
                                b = Convert.ToSingle(auxstr[0].Substring(3, 2), null); // minutos
                                c = Convert.ToSingle(auxstr[0].Substring(5, 2), null); // segundos
                                d = 0;
                                if (lengthW > 8)
                                    d = Convert.ToSingle(auxstr[0].Substring(7, 3), null); // milisegundos  -- if applicable

                                //Obtaining the final value
                                y = a + (b / 60) + ((c + d / 1000) / 3600);

                                //Checking if should be positive or negative
                                if (auxstr[0].EndsWith("O", StringComparison.Ordinal) || auxstr[0].EndsWith("W", StringComparison.Ordinal))
                                    y = -1 * y;

                                pp.Add(new Point().LatLong2XY(x, y));
                            }
                            Polygons.Add(pp);

                            j += num;
                            break;

                        default:
                            j++;
                            break;
                    }
                }
                else j++;
            }
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

        public List<List<Point>> getPolygons()
        {
            return this.Polygons;
        }

        public List<Point> getPolygon(int i)
        {
            return this.Polygons[i];
        }

        public int getIndex(int i)
        {
            if (index[i] != null)
                return index[i];
            else
                return -1;
        }

        public List<Tuple<Point, string>> getSimbols()
        {
            return this.Simbols;
        }

        public List<Tuple<Point, string>> getTexts()
        {
            return this.Texts;
        }

        public Color GetColor(int index)
        {
            Color colorBrush;
            ListColorsPoligons.TryGetValue(index, out colorBrush);
            if (colorBrush != null)
                return colorBrush;
            else
                return Color.FromArgb(100,255,0,0);
        }
    }
}
