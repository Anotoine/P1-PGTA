using Microsoft.WindowsAPICodePack.Dialogs;
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

        //User options stuff
        Options UserOptions = new Options();

        //Decoding stuff
        List<Message> listMessages;
        List<ShowRow> listRow;

        //DB stuff
        List<AircraftDB> listPlaneDB;

        //Logic stuff
        Dictionary<string, string> paths = new Dictionary<string, string>() { { "File", @"" }, { "Maps", @"" }, { "DB", @"" } };

        //Mapping stuff
        List<Map> Maps;
        List<CheckBox> checkBoxes;

        //Info stuff
        List<Vehicle> VehiclesList;
        List<string> Vistos;
        bool relaunch = false;

        public MainWindow()
        {
            InitializeComponent();
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
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                EnsurePathExists = true,
                EnsureValidNames = true,
                Multiselect = false,
                Title = "Load the ASTERIX file..."
            };
            openFileDialog.Filters.Add(new CommonFileDialogFilter("ASTERIX files", "*.ast"));
            openFileDialog.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));


            //if (string.IsNullOrEmpty(TLoadFile.Text) || !File.Exists(TLoadFile.Text))
            //    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //else
            //openFileDialog.InitialDirectory = TLoadFile.Text;

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                // Giving the user the path that it was selected
                TLoadFile.Text = openFileDialog.FileName;

        }

        private void BLoadMaps_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                EnsurePathExists = true,
                EnsureValidNames = true,
                Multiselect = false,
                Title = "Load the maps folder..."
            };
            //openFileDialog.Filters.Add(new CommonFileDialogFilter("Map file", "*.map"));

            if (string.IsNullOrEmpty(TLoadMaps.Text) || !Directory.Exists(TLoadMaps.Text))
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                openFileDialog.InitialDirectory = TLoadMaps.Text;

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                // Giving the user the path that it was selected
                TLoadMaps.Text = openFileDialog.FileName;
        }

        private void BLoadDB_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                EnsurePathExists = true,
                EnsureValidNames = true,
                Multiselect = false,
                Title = "Load the Database file..."
            };
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Aircraft DB", "*.csv"));
            openFileDialog.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));

            if (string.IsNullOrEmpty(TLoadDB.Text) || !File.Exists(TLoadDB.Text))
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                openFileDialog.InitialDirectory = TLoadDB.Text;

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                // Giving the user the path that it was selected
                TLoadDB.Text = openFileDialog.FileName;
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
                listRow.Add(new ShowRow(m));
                i += length;

                (sender as BackgroundWorker).ReportProgress((int)(((i + 1) * 100 / list.Count) + 0.001));
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
                    Maps.Add(new Map(file));
                }
                catch
                {
                    MessageBox.Show("The file at: " + file + " could not be read. The file will be skipped.", "Error while reading.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    (sender as BackgroundWorker).ReportProgress((int)(((k + 1) * 100 / listfiles.Length) + 0.001));
                }
            }
        }

        private void Worker_DoWork_DB(object sender, DoWorkEventArgs e)
        {
            listPlaneDB = new List<AircraftDB>();
            using (var reader = new StreamReader((string)e.Argument))
            {
                while (!reader.EndOfStream)
                    listPlaneDB.Add(new AircraftDB(reader.ReadLine().Split(',')));
            }

            if (!(listRow == null))
            {
                foreach (ShowRow sr in listRow)
                {
                    sr.AddDBData(listPlaneDB);
                }
            }

            if (!(listMessages == null))
            {
                Vistos = new List<string>();
                VehiclesList = new List<Vehicle>();
                for (int i = 0; i < listMessages.Count; i++)
                {
                    Message m = listMessages[i];
                    if (Vistos.Contains(m.getAddressICAO()))
                    {
                        if (m.getTOD() >= VehiclesList[Vistos.IndexOf(m.getAddressICAO())].getLastTime().AddSeconds(UserOptions.Interval))
                            VehiclesList[Vistos.IndexOf(m.getAddressICAO())].AddPoint(m);
                    }
                    else
                    {
                        Vistos.Add(m.getAddressICAO());
                        VehiclesList.Add(new Vehicle(m));
                    }
                    (sender as BackgroundWorker).ReportProgress((int)(((i + 1) * 100 / listMessages.Count) + 0.001));
                }
            }
        }

        void worker_ProgressChanged_LoadFile(object sender, ProgressChangedEventArgs e)
        {
            PBLoadFile.Value = e.ProgressPercentage;
        }

        void worker_ProgressChanged_Maps(object sender, ProgressChangedEventArgs e)
        {
            PBLoadMaps.Value = e.ProgressPercentage;
        }

        void worker_ProgressChanged_DB(object sender, ProgressChangedEventArgs e)
        {
            PBLoadDB.Value = e.ProgressPercentage;
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
                LPBLoadMaps.Text = "Loading maps...";
        }

        private void SliderTime_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (CheckVehicles != null)
                CheckBoxClickVehicles(sender, e);
        }

        private void Interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UserOptions.Interval = SlInterval.Value;
        }

        private void PBLoadDB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PBLoadDB.Value >= 100)
                LPBLoadDB.Text = "Aircraft DB loaded!";
            else
                LPBLoadDB.Text = "Loading DB...";
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
            foreach (Map map in Maps)
            {
                //Creating the Checkbox to be checked
                CheckBox checkBox = new CheckBox();
                checkBox.Content = map.getName();
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
                Maps = new List<Map>();
                checkBoxes = new List<CheckBox>();

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
                        foreach (Tuple<Point, Point> l in Maps[i].getPoints())
                        {
                            Line l1 = new Line();
                            l1.StrokeThickness = 1;
                            l1.Stroke = UserOptions.MapsColor.MainColor;
                            l1.X1 = (l.Item1.X + A) / alpha;
                            l1.Y1 = (l.Item1.Y + B) / beta;

                            l1.X2 = (l.Item2.X + A) / alpha;
                            l1.Y2 = (l.Item2.Y + B) / beta;
                            LienzoMaps.Children.Add(l1);
                        }
                        foreach (List<Point> pl in Maps[i].getPolylines())
                        {
                            Polyline poly = new Polyline();
                            poly.StrokeThickness = 1;
                            poly.Stroke = UserOptions.MapsColor.MainColor;
                            PointCollection points = new PointCollection();
                            foreach (Point pp in pl)
                                points.Add(new System.Windows.Point((pp.X + A) / alpha, (pp.Y + B) / beta));
                            poly.Points = points;
                            LienzoMaps.Children.Add(poly);
                        }
                        foreach (Tuple<Point, string> txt in Maps[i].getTexts())
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = txt.Item2;
                            textBlock.Foreground = UserOptions.MapsColor.SecondaryColor;
                            Canvas.SetLeft(textBlock, (txt.Item1.X + 10 + A) / alpha);
                            Canvas.SetTop(textBlock, (txt.Item1.Y + 10 + B) / beta); 
                            LienzoMaps.Children.Add(textBlock);
                        }
                        foreach (Tuple<Point, string> sim in Maps[i].getSimbols())
                        { //Maybe a polygon to help diferenciate?
                            Ellipse SIM = new Ellipse();
                            SIM.Stroke = UserOptions.MapsColor.HighlightColor;
                            SIM.Fill = UserOptions.MapsColor.HighlightColor;
                            SIM.Width = 5;
                            SIM.Height = 5;
                            Canvas.SetLeft(SIM, ((sim.Item1.X + A) / alpha) - SIM.Width / 2);
                            Canvas.SetTop(SIM, ((sim.Item1.Y + B) / beta) - SIM.Height / 2);
                            LienzoMaps.Children.Add(SIM);
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
                Canvas.SetLeft(ARPpoint, ((ARP.X + A) / alpha) - ARPpoint.Width / 2);
                Canvas.SetTop(ARPpoint, ((ARP.Y + B) / beta) - ARPpoint.Height / 2);
                LienzoMaps.Children.Add(ARPpoint);
            }
        }

        private void CheckBoxClickVehicles(object sender, RoutedEventArgs e)
        {
            LienzoVehicles.Children.Clear();

            if (CheckVehicles != null)
            {
                if (CheckVehicles.IsChecked == true)
                {
                    if (VehiclesList != null)
                    {
                        if (MergingTypeRADAR.SelectedIndex == 0) //Polylines
                        {
                            foreach (Vehicle v in VehiclesList)
                            {
                                if (v.Type.Equals("Aircraft"))
                                {
                                    Polyline pl = new Polyline();
                                    PointCollection pp = new PointCollection();

                                    List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        pp.Add(new System.Windows.Point(list[i].X * alphaARP - AARP, list[i].Y * betaARP - BARP));
                                        pl.Tag = v.TrackN + "/" + i;
                                    }

                                    if (v.Callsign == "NONE")
                                        pl.Stroke = UserOptions.OtherColor;
                                    else if (v.Callsign.StartsWith("F"))
                                        pl.Stroke = UserOptions.VehiclesColor;
                                    else
                                        pl.Stroke = UserOptions.AircraftColor;

                                    pl.MouseUp += new MouseButtonEventHandler(PlaneClick);

                                    pl.Points = pp;
                                    LienzoVehicles.Children.Add(pl);
                                }
                            }
                        } else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                        {
                            foreach (Vehicle v in VehiclesList)
                            {
                                if (v.Type == "Aircraft")
                                {
                                    List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        Ellipse p0 = new Ellipse();

                                        if (v.Callsign == "NONE")
                                            p0.Stroke = Brushes.LightSkyBlue;
                                        else if (v.Callsign.StartsWith("F"))
                                            p0.Stroke = Brushes.White;
                                        else
                                            p0.Stroke = Brushes.Red;

                                        p0.StrokeThickness = 1;
                                        p0.Width = 2;
                                        p0.Height = p0.Width;
                                        p0.Tag = v.TrackN + "/" + i;
                                        p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                        LienzoVehicles.Children.Add(p0);

                                        Canvas.SetLeft(p0, (list[i].X * alphaARP) - AARP - p0.Width / 2);
                                        Canvas.SetTop(p0, (list[i].Y * betaARP) - BARP - p0.Height / 2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PlaneClick(object sender, RoutedEventArgs e)
        {
            string[] TagValue = (e.OriginalSource as FrameworkElement).Tag.ToString().Split('/');
            int trackN = Convert.ToInt32(TagValue[0]);
            int j = Convert.ToInt32(TagValue[1]);

            bool exit = false; int i = 0;
            while (!(exit || i >= VehiclesList.Count))
            {
                if (VehiclesList[i].TrackN == trackN)
                    exit = true;
                else
                    i++;
            }

            string str = "ICAO Addres: " + VehiclesList[i].ICAOaddress + "\n" + "Callsign: " + VehiclesList[i].Callsign + "\n"
                + "X: " + VehiclesList[i].Positions[j].X + "m\n" + "Y: " + VehiclesList[i].Positions[j].Y + "m";
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
