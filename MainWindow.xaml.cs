using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ASTERIX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Point ARP, zero0;
        double A, B, AARP, BARP, alpha, beta, alphaARP, betaARP, xA, yA;

        //Decoding stuff
        List<Message> listMessages;
        List<ShowRow> listRow;

        //DB stuff
        List<Tuple<string, string, string, string, string>> listPlaneDB;

        //Logic stuff
        Dictionary<string, string> paths = new Dictionary<string, string> (){{"File", @""}, {"Maps", @""}, {"DB", @""}};

        //Mapping stuff
        List<List<Tuple<Point, Point>>> mapsLines;
        List<List<List<Point>>> mapsPolylines;
        List<string> mapsNames;
        List<CheckBox> checkBoxes;

        //Info stuff
        List<Vehicle> VehiclesList;
        bool relaunch = false;

        public MainWindow()
        {
            InitializeComponent();


            zero0 = new Point().LatLong2XY(41.315300, 2.043297); //x y superior esquerra 
            ARP = new Point().LatLong2XY(41.296944, 2.078333); //ARP BCN airport

            //xyz de Lambert --> xyz LienzoMaps (amb origen de coordenades a d'alt a l'esquerra)
            A = -zero0.X;
            B = -zero0.Y;
            alpha = A / (LienzoMaps.ActualWidth / 2);
            beta = B / (LienzoMaps.ActualHeight / 2);

            //new
            xA = (ARP.X + A) / alpha;
            yA = (ARP.Y + B) / beta;
            AARP = -xA;
            BARP = -yA;
            alphaARP = AARP / -2887;
            betaARP = BARP / 2078;
        }

        private void BLoad_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Visible;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;

            LPosX.Visibility = Visibility.Hidden;
            LPosY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = true;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = false;
            WSettings.IsEnabled = false;
            LPosX.IsEnabled = false;
            LPosY.IsEnabled = false;
        }

        private void BRadar_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Visible;
            WTable.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;

            LPosX.Visibility = Visibility.Visible;
            LPosY.Visibility = Visibility.Visible;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = true;
            WTable.IsEnabled = false;
            WSettings.IsEnabled = false;
            LPosX.IsEnabled = true;
            LPosY.IsEnabled = true;
        }

        private void BTable_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Visible;
            WSettings.Visibility = Visibility.Hidden;

            LPosX.Visibility = Visibility.Hidden;
            LPosY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = true;
            WSettings.IsEnabled = false;
            LPosX.IsEnabled = false;
            LPosY.IsEnabled = false;
        }

        private void BSettings_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Visible;

            LPosX.Visibility = Visibility.Hidden;
            LPosY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = false;
            WSettings.IsEnabled = true;
            LPosX.IsEnabled = false;
            LPosY.IsEnabled = false;
        }

        private void BLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                // Giving the user the path that it was selected
                TLoadFile.Text = openFileDialog.FileName;
            }
        }

        private void BLoadMaps_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                // Giving the user the path that it was selected
                TLoadMaps.Text = openFileDialog.FileName;

            }
        }

        private void BLoadDB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                // Giving the user the path that it was selected
                TLoadDB.Text = openFileDialog.FileName;
            }
        }

        private void Worker_DoWork_LoadFile(object sender, DoWorkEventArgs e)
        {
            //Temporal list for reading
            List<string> list = new List<string>();

            //Actually reading the file
            byte[] fileBytes = File.ReadAllBytes((string)e.Argument);

            foreach (byte b in fileBytes)
            {
                list.Add(Convert.ToString(b, 16).PadLeft(2, '0'));
            }

            //And parsing it
            int i = 0;
            while (i < list.Count)
            {
                int length = Int32.Parse(list[i + 1] + list[i + 2], System.Globalization.NumberStyles.HexNumber);
                Message m = new Message(list.GetRange(i, length));

                listMessages.Add(m);
                listRow.Add(new ShowRow(m, listPlaneDB));
                i += length;

                (sender as BackgroundWorker).ReportProgress((int)(((i+1) * 100 / list.Count) + 0.001));
            }
        }

        private void Worker_DoWork_Maps(object sender, DoWorkEventArgs e)
        {
            var listfiles = Directory.GetFiles((string)e.Argument);

            for (int k = 0; k < listfiles.Length; k++)
            {
                string file = listfiles[k];
                try
                {
                    //Reading lines and creating Lists for Line and Polyline
                    string[] lines = File.ReadAllLines(file);
                    mapsNames.Add(file.Split('\\')[file.Split('\\').Length - 1]);
                    List<Tuple<Point,Point>> mapL = new List<Tuple<Point, Point>>();
                    List<List<Point>> mapP = new List<List<Point>>();

                    //Start to read the lines
                    int j = 0;
                    while (j < lines.Length)
                    {
                        if (!lines[j].StartsWith("#"))
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
                                mapL.Add(new Tuple<Point,Point> (tPoint[0], tPoint[1]));
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
                                    float d1 = Convert.ToSingle(l2[0].Substring(6, 3)); // milisegundos

                                    float x1 = a1 + (b1 / 60) + ((c1 + d1 / 1000) / 3600);

                                    float a2 = Convert.ToSingle(l2[1].Substring(0, 3)); // grados
                                    float b2 = Convert.ToSingle(l2[1].Substring(3, 2)); // minutos
                                    float c2 = Convert.ToSingle(l2[1].Substring(5, 2)); // segundos
                                    float d2 = Convert.ToSingle(l2[1].Substring(7, 3)); // milisegundos

                                    float x2 = a2 + (b2 / 60) + ((c2 + d2 / 1000) / 3600);

                                    pp.Add(new Point().LatLong2XY(x1, x2));
                                }
                                mapP.Add(pp);
                                j += num;
                            }
                            else
                                j++;
                        }
                        else
                            j++;
                    }
                    mapsLines.Add(mapL);
                    mapsPolylines.Add(mapP);
                    (sender as BackgroundWorker).ReportProgress((int)(((k+1) * 100 / listfiles.Length) + 0.001));

                    e.Result = listfiles.ToString();
                }
                catch
                {
                    //TODO: MessageBox saying the ones that could not be solved
                    MessageBox.Show("The file at: " + file + " could not be read. The file will be skipped.", "Error while reading.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Worker_DoWork_DB(object sender, DoWorkEventArgs e)
        {
            List<string> Vistos = new List<string>();

            if (!(listMessages == null))
            {
                VehiclesList = new List<Vehicle>();
                for (int i = 0; i < listMessages.Count; i++)
                {
                    Message m = listMessages[i];
                    if (Vistos.Contains(m.getAddressICAO()))
                    {
                        VehiclesList[Vistos.IndexOf(m.getAddressICAO())].AddPoint(m);
                    }
                    else
                    {
                        Vistos.Add(m.getAddressICAO());
                        VehiclesList.Add(new Vehicle(m));
                    }
                    (sender as BackgroundWorker).ReportProgress((int)(((i+1) * 100 / listMessages.Count) + 0.001));
                }
            }
        }

        void worker_ProgressChanged_LoadFile(object sender, ProgressChangedEventArgs e)
        {
            PBLoadFile.Value = e.ProgressPercentage;
        }

        private void PBLoadFile_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PBLoadFile.Value >= 100)
                LPBLoadFile.Text = "File loaded!";
            else
                LPBLoadFile.Text = "Loading file...";
        }

        private void PBLoadMaps_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PBLoadMaps.Value >= 100)
                LPBLoadMaps.Text = "Maps loaded!";
            else
                LPBLoadMaps.Text =  "Loading maps...";
        }

        private void PBLoadDB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PBLoadDB.Value >= 100)
                LPBLoadDB.Text = "Aircraft DB loaded!";
            else
                LPBLoadDB.Text = "Loading DB...";
        }

        void worker_ProgressChanged_Maps(object sender, ProgressChangedEventArgs e)
        {
            PBLoadMaps.Value = e.ProgressPercentage;
        }

        void worker_ProgressChanged_DB(object sender, ProgressChangedEventArgs e)
        {
            PBLoadDB.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleated_LoadFile(object sender, RunWorkerCompletedEventArgs e)
        {
            //Showing a MessageBox on the final solution
            //MessageBox.Show(listMessages.Count + " are loaded to the program.", "Loaded!", MessageBoxButton.OK, MessageBoxImage.Information);

            //Binding the data to the table
            Table.ItemsSource = listRow;

            //Relaunching DB to load the data into the Vehicle form
            relaunch = true;
            BLoadRefresh_Click(sender, new RoutedEventArgs());
        }

        private void worker_RunWorkerCompleated_Maps(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (string str in mapsNames)
            {
                //Creating the Checkbox to be checked
                CheckBox checkBox = new CheckBox();
                checkBox.Content = str;
                checkBox.FontSize = 12;
                checkBox.Foreground = Brushes.White;
                checkBox.Margin = new Thickness(10, 10, 10, 10);
                checkBox.Click += new RoutedEventHandler(CheckBoxClickMaps);
                checkBoxes.Add(checkBox);
                StackMaps.Children.Add(checkBox);
            }

            //Showing a MessageBox on the final solution
            //MessageBox.Show(mapsNames.Count + " are loaded to the program.", "Loaded!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void worker_RunWorkerCompleated_DB(object sender, RunWorkerCompletedEventArgs e)
        {
            //Showing a MessageBox on the final solution
            //MessageBox.Show(VehiclesList.Count + " are loaded to the program.", "Loaded!", MessageBoxButton.OK, MessageBoxImage.Information);
            CheckVehicles.Visibility = Visibility.Visible;
        }

        private void BLoadRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!TLoadFile.Text.Equals(paths["File"]) || TLoadFile.Equals(""))
            {
                paths["File"] = TLoadFile.Text;

                //Starting variables to be filled
                listMessages = new List<Message>();
                listRow = new List<ShowRow>();

                PBLoadFile.Value = 0;
                //Loading a worker to do the Decoding stuff
                using (BackgroundWorker workerFile = new BackgroundWorker { WorkerReportsProgress = true })
                {
                    workerFile.DoWork += Worker_DoWork_LoadFile;
                    workerFile.ProgressChanged += worker_ProgressChanged_LoadFile;
                    workerFile.RunWorkerCompleted += worker_RunWorkerCompleated_LoadFile;
                    workerFile.RunWorkerAsync(TLoadFile.Text);
                }
            }

            if (!TLoadMaps.Text.Equals(paths["Maps"]) || TLoadMaps.Equals(""))
            {
                paths["Maps"] = TLoadMaps.Text;

                //Starting variables to be filled
                mapsLines = new List<List<Tuple<Point, Point>>>();
                mapsPolylines = new List<List<List<Point>>>();
                checkBoxes = new List<CheckBox>();
                mapsNames = new List<string>();

                PBLoadMaps.Value = 0;
                //Loading a worker to do the Decoding stuff
                using (BackgroundWorker workerMaps = new BackgroundWorker { WorkerReportsProgress = true })
                {
                    workerMaps.DoWork += Worker_DoWork_Maps;
                    workerMaps.ProgressChanged += worker_ProgressChanged_Maps;
                    workerMaps.RunWorkerCompleted += worker_RunWorkerCompleated_Maps;
                    workerMaps.RunWorkerAsync(TLoadMaps.Text);
                }
            }

            //Check condition, not working properly
            if (!TLoadDB.Text.Equals(paths["DB"]) || TLoadDB.Equals("") || relaunch)
            {
                paths["DB"] = TLoadDB.Text;

                //Starting variables to be filled
                VehiclesList = new List<Vehicle>();

                PBLoadDB.Value = 0;
                relaunch = false;
                //Loading a worker to do the Decoding stuff
                using (BackgroundWorker workerDB = new BackgroundWorker { WorkerReportsProgress = true })
                {
                    workerDB.DoWork += Worker_DoWork_DB;
                    workerDB.ProgressChanged += worker_ProgressChanged_DB;
                    workerDB.RunWorkerCompleted += worker_RunWorkerCompleated_DB;
                    workerDB.RunWorkerAsync(TLoadDB.Text);
                }
            }
        }

        private void CheckBoxClickMaps(object sender, RoutedEventArgs e)
        {
            LienzoMaps.Children.Clear();
            if (!(checkBoxes == null))
            {
                for (int i = 0; i < checkBoxes.Count; i++)
                {
                    if (checkBoxes[i].IsChecked == true)
                    {
                        foreach (Tuple<Point, Point> l in mapsLines[i])
                        {
                            Line l1 = new Line();
                            l1.StrokeThickness = 1;
                            l1.Stroke = Brushes.ForestGreen;
                            l1.X1 = (l.Item1.X + A) / alpha;
                            l1.Y1 = (l.Item1.Y + B) / beta;

                            l1.X2 = (l.Item2.X + A) / alpha;
                            l1.Y2 = (l.Item2.Y + B) / beta;
                            LienzoMaps.Children.Add(l1);
                        }
                        foreach (List<Point> pl in mapsPolylines[i])
                        {
                            Polyline poly = new Polyline();
                            poly.StrokeThickness = 1;
                            poly.Stroke = Brushes.ForestGreen;
                            PointCollection points = new PointCollection();
                            foreach (Point pp in pl)
                                points.Add(new System.Windows.Point((pp.X + A) / alpha, (pp.Y + B) / beta));
                            poly.Points = points;
                            LienzoMaps.Children.Add(poly);
                        }
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
                LienzoMaps.Children.Add(ARPpoint);
                Canvas.SetLeft(ARPpoint, ((ARP.X + A) / alpha) - ARPpoint.Width / 2);
                Canvas.SetTop(ARPpoint, ((ARP.Y + B) / beta) - ARPpoint.Height / 2);
            }
        }

        private void CheckBoxClickVehicles(object sender, RoutedEventArgs e)
        {
            LienzoVehicles.Children.Clear();
            
            if (CheckVehicles.IsChecked == true)
            {
                if (!(VehiclesList == null))
                {
                    foreach (Vehicle v in VehiclesList)
                    {
                        foreach (Point p in v.GetPointsByDate(new DateTime().AddHours(28)))
                        {
                            if (v.Type == "Aircraft")
                            {
                                Ellipse p0 = new Ellipse();

                                if (v.Callsign == "NONE")
                                    p0.Stroke = Brushes.LightSkyBlue;
                                else if (v.Callsign.StartsWith("F"))
                                    p0.Stroke = Brushes.White;
                                else
                                    p0.Stroke = Brushes.Red;

                                p0.StrokeThickness = 1;
                                p0.Width = 5;
                                p0.Height = p0.Width;
                                p0.Tag = v.TrackN;
                                p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                LienzoVehicles.Children.Add(p0);

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
                                LienzoVehicles.Children.Add(p0);

                                Canvas.SetLeft(p0, p.X * alphaARP - AARP - p0.Width / 2);
                                Canvas.SetTop(p0, p.Y * betaARP - BARP - p0.Height / 2);
                            }
                        }
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
                else
                    i++;
            }

            string str = "ICAO Addres: " + VehiclesList[i].ICAOaddress + "\n" + "Callsign: " + VehiclesList[i].Callsign + "\n"
                + "X: " + VehiclesList[i].Positions[0].X + "m\n" + "Y: " + VehiclesList[i].Positions[0].Y + "m";
            MessageBox.Show(str, "TrackN: " + VehiclesList[i].TrackN);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {   //entre abans del load, no comprendo
            ARP = new Point().LatLong2XY(41.296944, 2.078333);

            alpha = A / (LienzoMaps.ActualWidth / 2);
            beta = B / (LienzoMaps.ActualHeight / 2);

            //new
            xA = (ARP.X + A) / alpha;
            yA = (ARP.Y + B) / beta;
            AARP = -xA;
            BARP = -yA;
            alphaARP = AARP / -2887;
            betaARP = BARP / 2078;

            CheckBoxClickVehicles(sender, e);
            CheckBoxClickMaps(sender, e);
        }

        private void LienzoMaps_MouseMove(object sender, MouseEventArgs e)
        {
            LPosX.Text = ((e.GetPosition(LienzoMaps).X + AARP) / alphaARP).ToString("0.###m");
            LPosY.Text = ((e.GetPosition(LienzoMaps).Y + BARP) / betaARP).ToString("0.###m");
        }

    }
}
