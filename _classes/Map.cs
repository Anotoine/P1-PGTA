using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ideafix
{
    class Map
    {
        string Name;
        
        List<Tuple<Point, Point>> Lines = new List<Tuple<Point, Point>>();
        List<Tuple<Point, string>> Simbols = new List<Tuple<Point, string>>();
        List<Tuple<Point, string>> Texts = new List<Tuple<Point, string>>();
        List<List<Point>> Polylines = new List<List<Point>>();

        Color SimbolsColor = new Color();
        Color TextsColor = new Color();
        Color LinesColor = new Color();

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
                int lengthN, lengthW;
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
                            TextsColor = Color.FromArgb(Convert.ToInt32(auxstr[0], 10), Convert.ToInt32(auxstr[1], 10), Convert.ToInt32(auxstr[2], 10));
                            
                            j++;
                            break;

                        case "ColorSimbolos":
                            auxstr = lines[j].Substring(13).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            SimbolsColor = Color.FromArgb(Convert.ToInt32(auxstr[0], 10), Convert.ToInt32(auxstr[1], 10), Convert.ToInt32(auxstr[2], 10));

                            j++;
                            break;

                        case "ColorLinea":
                            auxstr = lines[j].Substring(10).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            LinesColor = Color.FromArgb(Convert.ToInt32(auxstr[0], 10), Convert.ToInt32(auxstr[1], 10), Convert.ToInt32(auxstr[2], 10));

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
                            int num = Convert.ToInt32(lines[j].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1], 10);
                            List<Point> pp = new List<Point>();

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

        public List<Tuple<Point, string>> getSimbols()
        {
            return this.Simbols;
        }

        public List<Tuple<Point, string>> getTexts()
        {
            return this.Texts;
        }
    }
}
