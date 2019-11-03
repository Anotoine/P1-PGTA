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
                        if (m.getTOD() >= VehiclesList[Vistos.IndexOf(m.getAddressICAO())].getLastTime().AddSeconds(1))
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
                            l1.Stroke = UserOptions.MapsColor;
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
                            poly.Stroke = UserOptions.MapsColor;
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
                if (VehiclesList != null)
                {
                    foreach (Vehicle v in VehiclesList)
                    {
                        if (v.Type.Equals("Aircraft"))
                        {
                            PointCollection pp = new PointCollection();

                            Polyline pl = new Polyline();

                            if (v.Callsign == "NONE")
                                pl.Stroke = UserOptions.OtherColor;
                            else if (v.Callsign.StartsWith("F"))
                                pl.Stroke = UserOptions.VehiclesColor;
                            else
                                pl.Stroke = UserOptions.AircraftColor;

                            pl.Tag = v.TrackN;
                            pl.MouseUp += new MouseButtonEventHandler(PlaneClick);

                            foreach (Point p in v.GetPointsByRangeDate(new DateTime().AddHours(SlStart.Value), new DateTime().AddHours(SlStop.Value)))
                            {
                                pp.Add(new System.Windows.Point(p.X * alphaARP - AARP, p.Y * betaARP - BARP));
                            }

                            pl.Points = pp;
                            LienzoVehicles.Children.Add(pl);
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
