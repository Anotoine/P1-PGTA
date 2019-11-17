using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ideafix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //Point ARP, zero0;
        Point zero0;
        Point mouseP = new Point();
        //double A, B, AARP, BARP, alpha, beta, alphaARP, betaARP, xA, yA, xe, yn;
        double A, B, alpha, beta;

        //User options stuff
        Options UserOptions = new Options();

        //Decoding stuff
        List<Message> listMessages;
        List<ShowRow> listRow;
        List<ShowPerf> listPerf;

        //DB stuff
        List<AircraftDB> listPlaneDB;

        //Logic stuff
        Dictionary<string, string> paths = new Dictionary<string, string>() { { "File", @"" }, { "Maps", @"" }, { "DB", @"" } };
        bool Isv023 = false;

        //Mapping stuff
        List<Map> Maps;
        List<CheckBox> checkBoxes;

        //Info stuff
        List<Vehicle> VehiclesList;
        List<string> Vistos;
        bool relaunch = false;

        //Timer
        DispatcherTimer timer = new DispatcherTimer();
        DateTime ActualTime;
        int Estela = 10;

        //Performance
        internal double[] pUD { get; set; } //UPDATE
        internal double[] pMD { get; set; } //MLAT DETECTION
        internal string[] Place = new string[] { "MANEUVERING AREA", "STAND", "APRON" };

        public MainWindow()
        {
            DataContext = UserOptions;
            InitializeComponent();
        }

        private void BLoad_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Visible;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WPerformance.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;
            WUPC.Visibility = Visibility.Hidden;

            if (string.IsNullOrEmpty(TLoadFile.Text))
                TLoadFile.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Ideafix\Test Files\SMR_MLAT_cat10_ADSB_Cat21_v023\160510-lebl-220001.ast";
            if (string.IsNullOrEmpty(TLoadMaps.Text))
                TLoadMaps.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Ideafix\Maps";
            if (string.IsNullOrEmpty(TLoadDB.Text))
                TLoadDB.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Ideafix\Database Files\acrft_db.csv";

            LPosLL.Visibility = Visibility.Hidden;
            LPosXY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = true;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = false;
            WPerformance.IsEnabled = false;
            WSettings.IsEnabled = false;
            WUPC.IsEnabled = false;
            LPosLL.IsEnabled = false;
            LPosXY.IsEnabled = false;
        }

        private void BRadar_Click(object sender, RoutedEventArgs e)
        {
            zero0 = new Point().LatLong2XY(41.315955, 2.028508);

            //xyz de Lambert --> xyz LienzoMaps (amb origen de coordenades a d'alt a l'esquerra)
            A = -zero0.X;
            B = -zero0.Y;
            alpha = A / (LienzoMaps.ActualWidth / 2);
            beta = B / (LienzoMaps.ActualHeight / 2);

            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Visible;
            WTable.Visibility = Visibility.Hidden;
            WPerformance.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;
            WUPC.Visibility = Visibility.Hidden;

            LPosLL.Visibility = Visibility.Visible;
            LPosXY.Visibility = Visibility.Visible;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = true;
            WTable.IsEnabled = false;
            WPerformance.IsEnabled = false;
            WSettings.IsEnabled = false;
            WUPC.IsEnabled = false;
            LPosLL.IsEnabled = true;
            LPosXY.IsEnabled = true;
        }

        private void BTable_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Visible;
            WPerformance.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;
            WUPC.Visibility = Visibility.Hidden;

            LPosLL.Visibility = Visibility.Hidden;
            LPosXY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = true;
            WPerformance.IsEnabled = false;
            WSettings.IsEnabled = false;
            WUPC.IsEnabled = false;
            LPosLL.IsEnabled = false;
            LPosXY.IsEnabled = false;
        }

        private void BPerfo_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WPerformance.Visibility = Visibility.Visible;
            WSettings.Visibility = Visibility.Hidden;
            WUPC.Visibility = Visibility.Hidden;

            LPosLL.Visibility = Visibility.Hidden;
            LPosXY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = false;
            WPerformance.IsEnabled = true;
            WSettings.IsEnabled = false;
            WUPC.IsEnabled = false;
            LPosLL.IsEnabled = false;
            LPosXY.IsEnabled = false;
        }

        private void BSettings_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WPerformance.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Visible;
            WUPC.Visibility = Visibility.Hidden;

            LPosLL.Visibility = Visibility.Hidden;
            LPosXY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = false;
            WSettings.IsEnabled = true;
            WUPC.IsEnabled = false;
            LPosLL.IsEnabled = false;
            LPosXY.IsEnabled = false;
        }

        private void BUPC_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WPerformance.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;
            WUPC.Visibility = Visibility.Visible;

            LPosLL.Visibility = Visibility.Hidden;
            LPosXY.Visibility = Visibility.Hidden;

            WLoad.IsEnabled = false;
            WRadar.IsEnabled = false;
            WTable.IsEnabled = false;
            WPerformance.IsEnabled = false;
            WSettings.IsEnabled = false;
            WUPC.IsEnabled = true;
            LPosLL.IsEnabled = false;
            LPosXY.IsEnabled = false;
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


            if (string.IsNullOrEmpty(TLoadFile.Text) || !File.Exists(TLoadFile.Text))
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
            openFileDialog.InitialDirectory = TLoadFile.Text;

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

        private void BLoadRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (CAT21v023Sel.IsChecked == true)
                Isv023 = true;
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

        private void BPause_Click(object seder, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
                timer.Stop();

            UserOptions.ActualTime = ActualTime.ToString();
        }

        private void BPlay_Click(object seder, RoutedEventArgs e)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                    timer.Stop();
                else
                {
                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(Math.Pow(SlSpeed.Value, -1));
                    timer.Tick += Next_Tick;
                    timer.Start();
                    ActualTime = new DateTime().AddHours(SlTime.LowerValue);
                }
            }
            else
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(Math.Pow(SlSpeed.Value, -1));
                timer.Tick += Next_Tick;
                timer.Start();
                ActualTime = new DateTime().AddHours(SlTime.LowerValue);
            }
        }

        private void Next_Tick(object sender, EventArgs e)
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

                            for (int j = 0; j < VehiclesList.Count; j++)
                            {
                                Vehicle v = VehiclesList[j];
                                bool exit = false;
                                int i = 0;
                                if (v.Callsign.StartsWith("F"))
                                {
                                    Polyline pl = new Polyline();
                                    PointCollection pp = new PointCollection();

                                    List<Point> listP = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    List<DateTime> listT = v.GetTimesByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    while (!exit && i < listP.Count)
                                    {
                                        if (listP[i] != null)
                                        {
                                            if (DateTime.Compare(listT[i], ActualTime) < 0)
                                                pp.Add(new System.Windows.Point((listP[i].X + A) / alpha, (listP[i].Y + B) / beta));


                                            if (DateTime.Compare(listT[i], ActualTime) > 0) //Check if final reach
                                                exit = true;
                                        }
                                        i++;
                                    }
                                    pl.Stroke = UserOptions.VehiclesColor;
                                    pl.MouseUp += new MouseButtonEventHandler(PlaneClick);

                                    pl.Points = pp;
                                    LienzoVehicles.Children.Add(pl);
                                }
                            }
                        }
                        else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                        {
                            for (int j = 0; j < VehiclesList.Count; j++)
                            {
                                Vehicle v = VehiclesList[j];
                                bool exit = false;
                                int i = 0;

                                if (v.Callsign.StartsWith("F"))
                                {
                                    List<Point> listP = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    List<DateTime> listT = v.GetTimesByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    while (!exit && i < listP.Count)
                                    {
                                        Ellipse p0 = new Ellipse();
                                        p0.Stroke = UserOptions.VehiclesColor;

                                        p0.StrokeThickness = 1;
                                        p0.Width = 2;
                                        p0.Height = p0.Width;

                                        p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                        p0.Tag = j + "/" + i;
                                        LienzoVehicles.Children.Add(p0);

                                        Canvas.SetLeft(p0, (listP[i].X + A) / alpha - p0.Width / 2);
                                        Canvas.SetTop(p0, (listP[i].Y + B) / beta - p0.Height / 2);

                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (CheckTraffic != null)
            {
                if (CheckTraffic.IsChecked == true)
                {
                    if (VehiclesList != null)
                    {
                        if (MergingTypeRADAR.SelectedIndex == 0) //Polylines
                        {

                            for (int j = 0; j < VehiclesList.Count; j++)
                            {
                                Vehicle v = VehiclesList[j];
                                bool exit = false;
                                int i = 0;
                                if (v.Callsign != "NONE" && !v.Callsign.StartsWith("F"))
                                {
                                    Polyline pl = new Polyline();
                                    PointCollection pp = new PointCollection();

                                    List<Point> listP = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    List<DateTime> listT = v.GetTimesByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    while (!exit && i < listP.Count)
                                    {
                                        if (listP[i] != null)
                                        {
                                            if (DateTime.Compare(listT[i], ActualTime) < 0)
                                                pp.Add(new System.Windows.Point((listP[i].X + A) / alpha, (listP[i].Y + B) / beta));


                                            if (DateTime.Compare(listT[i], ActualTime) > 0) //Check if final reach
                                                exit = true;
                                        }
                                        i++;
                                    }
                                    pl.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                    pl.Stroke = UserOptions.AircraftColor;
                                    pl.Points = pp;
                                    LienzoVehicles.Children.Add(pl);
                                }
                            }
                        }
                        else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                        {
                            for (int j = 0; j < VehiclesList.Count; j++)
                            {
                                Vehicle v = VehiclesList[j];
                                bool exit = false;
                                int i = 0;

                                if (v.Callsign != "NONE" && !v.Callsign.StartsWith("F"))
                                {
                                    List<Point> listP = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    List<DateTime> listT = v.GetTimesByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    while (!exit && i < listP.Count)
                                    {
                                        Ellipse p0 = new Ellipse();
                                        p0.Stroke = UserOptions.VehiclesColor;
                                        p0.MouseUp += new MouseButtonEventHandler(PlaneClick);

                                        p0.StrokeThickness = 1;
                                        p0.Width = 2;
                                        p0.Height = p0.Width;
                                        p0.Tag = j + "/" + i;
                                        LienzoVehicles.Children.Add(p0);

                                        Canvas.SetLeft(p0, (listP[i].X + A) / alpha - p0.Width / 2);
                                        Canvas.SetTop(p0, (listP[i].Y + B) / beta - p0.Height / 2);

                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (CheckUnknown != null)
            {
                if (CheckUnknown.IsChecked == true)
                {
                    if (VehiclesList != null)
                    {
                        if (MergingTypeRADAR.SelectedIndex == 0) //Polylines
                        {
                            for (int j = 0; j < VehiclesList.Count; j++)
                            {
                                Vehicle v = VehiclesList[j];
                                bool exit = false;
                                int i = 0;
                                if (v.Callsign == "NONE")
                                {
                                    Polyline pl = new Polyline();
                                    PointCollection pp = new PointCollection();

                                    List<Point> listP = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    List<DateTime> listT = v.GetTimesByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    while (!exit && i < listP.Count)
                                    {
                                        if (listP[i] != null)
                                        {
                                            if (DateTime.Compare(listT[i], ActualTime) < 0)
                                                pp.Add(new System.Windows.Point((listP[i].X + A) / alpha, (listP[i].Y + B) / beta));


                                            if (DateTime.Compare(listT[i], ActualTime) > 0) //Check if final reach
                                                exit = true;
                                        }
                                        i++;
                                    }
                                    pl.Stroke = UserOptions.OtherColor;
                                    pl.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                    pl.Points = pp;
                                    LienzoVehicles.Children.Add(pl);
                                }
                            }
                        }
                        else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                        {
                            for (int j = 0; j < VehiclesList.Count; j++)
                            {
                                Vehicle v = VehiclesList[j];
                                bool exit = false;
                                int i = 0;

                                if (v.Callsign == "NONE")
                                {
                                    List<Point> listP = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    List<DateTime> listT = v.GetTimesByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                    while (!exit && i < listP.Count)
                                    {
                                        Ellipse p0 = new Ellipse();
                                        p0.Stroke = UserOptions.VehiclesColor;

                                        p0.StrokeThickness = 1;
                                        p0.Width = 2;
                                        p0.Height = p0.Width;
                                        p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                        p0.Tag = j + "/" + i;
                                        LienzoVehicles.Children.Add(p0);

                                        Canvas.SetLeft(p0, (listP[i].X + A) / alpha - p0.Width / 2);
                                        Canvas.SetTop(p0, (listP[i].Y + B) / beta - p0.Height / 2);

                                    }
                                }
                            }
                        }
                    }
                }
            }

            ActualTime = ActualTime.AddSeconds(1);
            UserOptions.ActualTime = ActualTime.TimeOfDay.ToString();
        }

        private void Worker_DoWork_LoadFile(object sender, DoWorkEventArgs e)
        {
            //Temporal list for reading
            List<string> list = new List<string>();

            try
            {
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
                    Message m = new Message(list.GetRange(i, length), Isv023);

                    listMessages.Add(m);
                    listRow.Add(new ShowRow(m));
                    i += length;

                    (sender as BackgroundWorker).ReportProgress((int)(((i + 1) * 100 / list.Count) + 0.001));
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Could not open the file: " + (string)e.Argument + "\nCheck permissions and try again.", "Error while opening.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("It was not possible to read the file: " + (string)e.Argument + "\nTry with a diferent file.", "Error while reading and decoding.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void Worker_DoWork_Maps(object sender, DoWorkEventArgs e)
        {
            string[] listfiles;
            if (Directory.Exists((string)e.Argument))
            {
                listfiles = Directory.GetFiles((string)e.Argument);
                if (listfiles.Length > 0)
                {
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
                else
                    MessageBox.Show("No files found at: " + (string)e.Argument + ".\nThe directory will be skipped.", "Error not reading any file.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Directory not found (" + (string)e.Argument + ").", "Error directory does not exist.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Worker_DoWork_DB(object sender, DoWorkEventArgs e)
        {
            listPlaneDB = new List<AircraftDB>();
            try
            {
                using (var reader = new StreamReader((string)e.Argument))
                {
                    while (!reader.EndOfStream)
                        listPlaneDB.Add(new AircraftDB(reader.ReadLine().Split(',')));
                }
            }
            catch
            {
                MessageBox.Show("It was not possible to read the file: " + (string)e.Argument + "\nThe DB maybe corrupted.", "Error while reading.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (!(listRow == null))
                {
                    foreach (ShowRow sr in listRow)
                    {
                        sr.AddDBData(listPlaneDB);
                    }
                }
            }
            catch
            {
                MessageBox.Show("It was not possible to add the database data to the table", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
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

                    for (int i = 0; i < VehiclesList.Count; i++)
                    {
                        foreach (Point p in VehiclesList[i].GetPositions())
                        {
                            bool exit = false;
                            for (int j = 0; j < Maps.Count; j++)
                            {
                                int k = 0;
                                while (!exit && k < Maps[j].getPolygons().Count)
                                {
                                    if (IsPointInPolygon4(Maps[j].getPolygons()[k], p))
                                    {
                                        VehiclesList[i].Place.Add(Maps[j].getIndex(k));
                                        exit = true;
                                    }
                                    k++;
                                }
                            }
                            if (!exit)
                                VehiclesList[i].Place.Add(0);
                        }
                    }
                    //PERFORMANCE ESTEL
                    if (VehiclesList != null)
                        if (VehiclesList.Count > 0)
                            Performance(VehiclesList);
                }

            }
            catch
            {
                MessageBox.Show("It was not possible to add the information from the Database.", "Error comparing.", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void worker_RunWorkerCompleated_LoadFile(object sender, RunWorkerCompletedEventArgs e)
        {
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
                CheckBox checkBox = new CheckBox
                {
                    Content = map.getName(),
                    FontSize = 12,
                    Foreground = UserOptions.MapTextColor,
                    Margin = new Thickness(10, 10, 10, 10)
                };
                checkBox.Click += new RoutedEventHandler(CheckBoxClickMaps);
                checkBoxes.Add(checkBox);
                StackMaps.Children.Add(checkBox);
            }
        }

        private void worker_RunWorkerCompleated_DB(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckTraffic.Visibility = Visibility.Visible;
            CheckVehicles.Visibility = Visibility.Visible;
            CheckUnknown.Visibility = Visibility.Visible;

            TPerfo.ItemsSource = listPerf;
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

        private void PBLoadDB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PBLoadDB.Value >= 100)
                LPBLoadDB.Text = "Aircraft DB loaded!";
            else
                LPBLoadDB.Text = "Loading DB...";
        }

        private void SliderTime_ValueChanged(object sender, RoutedEventArgs e)
        {
            CheckBoxClickAll(sender, e);
        }

        private void Interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UserOptions.Interval = SlInterval.Value;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (checkBoxes != null)
            {
                double[] zoom = new double[] { 41.315955, 2.028508, 41.393904, 1.842814, 42.115028, 0.005309, 43.542697, -3.904945 };

                int a = Convert.ToInt32(e.NewValue * 2);
                double lat = zoom[a - 2];
                double lon = zoom[a - 1];
                zero0 = new Point().LatLong2XY(lat, lon);

                A = -zero0.X;
                B = -zero0.Y;
                alpha = A / (LienzoMaps.ActualWidth / 2);
                beta = B / (LienzoMaps.ActualHeight / 2);

                CheckBoxClickAll(sender, e);
                CheckBoxClickMaps(sender, e);
            }
        }

        private void BZoomout_Click(object sender, RoutedEventArgs e)
        {
            SlZoom.Value++;
        }

        private void BZoomIn_Click(object sender, RoutedEventArgs e)
        {
            SlZoom.Value--;
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
                            Line l1 = new Line
                            {
                                StrokeThickness = 1,
                                Stroke = UserOptions.MapMainColor,
                                X1 = (l.Item1.X + A) / alpha,
                                Y1 = (l.Item1.Y + B) / beta,

                                X2 = (l.Item2.X + A) / alpha,
                                Y2 = (l.Item2.Y + B) / beta
                            };
                            LienzoMaps.Children.Add(l1);
                        }
                        foreach (List<Point> pl in Maps[i].getPolylines())
                        {
                            Polyline poly = new Polyline
                            {
                                StrokeThickness = 1,
                                Stroke = UserOptions.MapMainColor
                            };
                            PointCollection points = new PointCollection();
                            foreach (Point pp in pl)
                                points.Add(new System.Windows.Point((pp.X + A) / alpha, (pp.Y + B) / beta));
                            poly.Points = points;
                            LienzoMaps.Children.Add(poly);
                        }
                        for (int j = 0; j < Maps[i].getPolygons().Count; j++)
                        {
                            List<Point> pl = Maps[i].getPolygon(j);
                            int index = Maps[i].getIndex(j);
                            Polygon pol = new Polygon
                            {
                                StrokeThickness = 1,
                                Stroke = new SolidColorBrush(Maps[i].GetColor(index)),
                                Fill = new SolidColorBrush(Maps[i].GetColor(index))
                            };
                            PointCollection points = new PointCollection();
                            foreach (Point pp in pl)
                                points.Add(new System.Windows.Point((pp.X + A) / alpha, (pp.Y + B) / beta));
                            pol.Points = points;
                            LienzoMaps.Children.Add(pol);

                        }
                        foreach (Tuple<Point, string> txt in Maps[i].getTexts())
                        {
                            TextBlock textBlock = new TextBlock
                            {
                                Text = txt.Item2,
                                Foreground = UserOptions.MapSecondaryColor
                            };
                            Canvas.SetLeft(textBlock, (txt.Item1.X + 10 + A) / alpha);
                            Canvas.SetTop(textBlock, (txt.Item1.Y + 10 + B) / beta);
                            LienzoMaps.Children.Add(textBlock);
                        }
                        foreach (Tuple<Point, string> sim in Maps[i].getSimbols())
                        {
                            Ellipse SIM = new Ellipse
                            {
                                Stroke = UserOptions.MapHighlightColor,
                                Fill = UserOptions.MapHighlightColor,
                                Width = 5,
                                Height = 5
                            };
                            Canvas.SetLeft(SIM, ((sim.Item1.X + A) / alpha) - SIM.Width / 2);
                            Canvas.SetTop(SIM, ((sim.Item1.Y + B) / beta) - SIM.Height / 2);
                            LienzoMaps.Children.Add(SIM);
                        }
                    }
                }
            }
        }

        private void CheckBoxClickAll(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
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
                                for (int j = 0; j < VehiclesList.Count; j++)
                                {
                                    Vehicle v = VehiclesList[j];
                                    if (v.Callsign.StartsWith("F"))
                                    {
                                        Polyline pl = new Polyline();
                                        PointCollection pp = new PointCollection();

                                        List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (list[i] != null)
                                            {
                                                pp.Add(new System.Windows.Point((list[i].X + A) / alpha, (list[i].Y + B) / beta));
                                                pl.Tag = j + "/" + i;
                                            }
                                        }

                                        pl.Stroke = UserOptions.VehiclesColor;
                                        pl.MouseUp += new MouseButtonEventHandler(PlaneClick);

                                        pl.Points = pp;
                                        LienzoVehicles.Children.Add(pl);
                                    }

                                }
                            }
                            else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                            {
                                for (int j = 0; j < VehiclesList.Count; j++)
                                {
                                    Vehicle v = VehiclesList[j];
                                    if (v.Callsign.StartsWith("F"))
                                    {
                                        List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (list[i] != null)
                                            {
                                                Ellipse p0 = new Ellipse();
                                                p0.Stroke = UserOptions.VehiclesColor;

                                                p0.StrokeThickness = 1;
                                                p0.Width = 2;
                                                p0.Height = p0.Width;
                                                p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                                p0.Tag = j + "/" + i;
                                                LienzoVehicles.Children.Add(p0);

                                                Canvas.SetLeft(p0, (list[i].X + A) / alpha - p0.Width / 2);
                                                Canvas.SetTop(p0, (list[i].Y + B) / beta - p0.Height / 2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (CheckTraffic != null)
                {
                    if (CheckTraffic.IsChecked == true)
                    {
                        if (VehiclesList != null)
                        {
                            if (MergingTypeRADAR.SelectedIndex == 0) //Polylines
                            {
                                for (int j = 0; j < VehiclesList.Count; j++)
                                {
                                    Vehicle v = VehiclesList[j];
                                    if (v.Callsign != "NONE" && !v.Callsign.StartsWith("F"))
                                    {
                                        Polyline pl = new Polyline();
                                        PointCollection pp = new PointCollection();

                                        List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (list[i] != null)
                                            {
                                                pp.Add(new System.Windows.Point((list[i].X + A) / alpha, (list[i].Y + B) / beta));
                                                pl.Tag = j + "/" + i;
                                            }
                                        }
                                        pl.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                        pl.Stroke = UserOptions.AircraftColor;

                                        pl.Points = pp;
                                        LienzoVehicles.Children.Add(pl);
                                    }
                                }
                            }
                            else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                            {
                                for (int j = 0; j < VehiclesList.Count; j++)
                                {
                                    Vehicle v = VehiclesList[j];
                                    if (v.Callsign != "NONE" && !v.Callsign.StartsWith("F"))
                                    {
                                        List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (list[i] != null)
                                            {
                                                Ellipse p0 = new Ellipse();

                                                p0.StrokeThickness = 1;
                                                p0.Width = 2;
                                                p0.Height = p0.Width;
                                                p0.Stroke = UserOptions.AircraftColor;
                                                p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                                p0.Tag = j + "/" + i;
                                                LienzoVehicles.Children.Add(p0);

                                                Canvas.SetLeft(p0, (list[i].X + A) / alpha - p0.Width / 2);
                                                Canvas.SetTop(p0, (list[i].Y + B) / beta - p0.Height / 2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (CheckUnknown != null)
                {
                    if (CheckUnknown.IsChecked == true)
                    {
                        if (VehiclesList != null)
                        {
                            if (MergingTypeRADAR.SelectedIndex == 0) //Polylines
                            {
                                for (int j = 0; j < VehiclesList.Count; j++)
                                {
                                    Vehicle v = VehiclesList[j];
                                    if (v.Callsign == "NONE")
                                    {
                                        Polyline pl = new Polyline();

                                        PointCollection pp = new PointCollection();

                                        List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (list[i] != null)
                                            {
                                                pp.Add(new System.Windows.Point((list[i].X + A) / alpha, (list[i].Y + B) / beta));
                                                pl.Tag = j + "/" + i;
                                            }
                                        }
                                        pl.Stroke = UserOptions.OtherColor;
                                        pl.MouseUp += new MouseButtonEventHandler(PlaneClick);

                                        pl.Points = pp;
                                        LienzoVehicles.Children.Add(pl);
                                    }
                                }
                            }
                            else if (MergingTypeRADAR.SelectedIndex == 1) //Points
                            {
                                for (int j = 0; j < VehiclesList.Count; j++)
                                {
                                    Vehicle v = VehiclesList[j];
                                    if (v.Callsign == "NONE")
                                    {
                                        List<Point> list = v.GetPointsByRangeDate(new DateTime().AddHours(SlTime.LowerValue), new DateTime().AddHours(SlTime.HigherValue));
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (list[i] != null)
                                            {
                                                Ellipse p0 = new Ellipse();
                                                p0.Stroke = UserOptions.OtherColor;

                                                p0.StrokeThickness = 1;
                                                p0.Width = 2;
                                                p0.Height = p0.Width;
                                                p0.MouseUp += new MouseButtonEventHandler(PlaneClick);
                                                p0.Tag = j + "/" + i;
                                                LienzoVehicles.Children.Add(p0);

                                                Canvas.SetLeft(p0, (list[i].X + A) / alpha - p0.Width / 2);
                                                Canvas.SetTop(p0, (list[i].Y + B) / beta - p0.Height / 2);
                                            }
                                        }
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
            int j = Convert.ToInt32(TagValue[0]);
            int i = Convert.ToInt32(TagValue[1]);

            string str = "ICAO Addres: " + VehiclesList[j].ICAOaddress + "\n" + "Callsign: " + VehiclesList[j].Callsign + "\n"
                + "X: " + VehiclesList[j].Positions[i].X + "m\n" + "Y: " + VehiclesList[j].Positions[i].Y + "m";
            MessageBox.Show(str, "TrackN: " + VehiclesList[j].TrackN);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            alpha = A / (LienzoMaps.ActualWidth / 2);
            beta = B / (LienzoMaps.ActualHeight / 2);

            CheckBoxClickAll(sender, e);
            CheckBoxClickMaps(sender, e);
        }

        private void LienzoVehicles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (CheckBox CB in checkBoxes)
            {
                if (CB.Content.ToString() == "MOPS")
                {
                    if (CB.IsChecked == true)
                    {
                        bool exit = false;
                        for (int j = 0; j < Maps.Count; j++)
                        {
                            int k = 0;
                            while (!exit && k < Maps[j].getPolygons().Count)
                            {
                                Point p = new Point();
                                p.X = e.GetPosition(LienzoMaps).X * alpha - A;
                                p.Y = e.GetPosition(LienzoMaps).Y * beta - B;
                                if (IsPointInPolygon4(Maps[j].getPolygons()[k], p))
                                {
                                    int n = Maps[j].getIndex(k) - 1;
                                    MessageBox.Show(Place[n] + "\n" + "Probability of Update:" + "\n" + pUD[n].ToString() + "%" +
                                        "\n Probability of MLAT detection: \n" + pMD[n] + "%");
                                    //VehiclesList[i].Place.Add(Maps[j].getIndex(k));
                                    exit = true;
                                }
                                k++;
                            }
                        }

                        //IsPointInPolygon4(List < Point > polygon, Point testPoint)
                    }
                }
            }
        }

        private void LienzoMaps_MouseMove(object sender, MouseEventArgs e)
        {
            mouseP.XY2LatLong(e.GetPosition(LienzoMaps).X * alpha - A, e.GetPosition(LienzoMaps).Y * beta - B);
            //LPosX.Text = (e.GetPosition(LienzoMaps).X *alpha - A).ToString("0.###m");
            //LPosY.Text = (e.GetPosition(LienzoMaps).Y*beta -B).ToString("0.###m");

            LPosLL.Text = "(Lat,Lon) = (" + (mouseP.DMSlat[0]).ToString("0º") + (mouseP.DMSlat[1]).ToString("0") + "'" + (mouseP.DMSlat[2]).ToString("0.###") + "'' ," +
                (mouseP.DMSlon[0]).ToString("0º") + (mouseP.DMSlon[1]).ToString("0") + "'" + (mouseP.DMSlon[2]).ToString("0.###") + "'')";
            LPosXY.Text = "- (x,y) = (" + (e.GetPosition(LienzoMaps).X * alpha - A).ToString("0.###m") + "," + (e.GetPosition(LienzoMaps).Y * beta - B).ToString("0.###m") + ")";

        }


        private static bool IsPointInPolygon4(List<Point> polygon, Point testPoint)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        //PERFORMANCE ESTEL
        private void Performance(List<Vehicle> vl)
        {
            listPerf = new List<ShowPerf>();

            //////UPDATE 222222222 and MLAT DET
            pUD = new double[] { 0, 0, 0 };
            pMD = new double[] { 0, 0, 0 };
            double[] winT = new double[] { 0, 0 };
            double[] winOK = new double[] { 0, 0 };
            double[] samplesT = new double[] { 0, 0, 0 };
            double[] TotalSecT = new double[] { 0, 0, 0 };
            foreach (Vehicle v in vl)
            {
                v.Performance();
                for (int i = 0; i < v.incSamples.Length; i++)
                {
                    samplesT[i] = samplesT[i] + v.incSamples[i];
                    TotalSecT[i] = TotalSecT[i] + v.TotalSec[i];
                }
                for (int i = 0; i < v.winOK.Length; i++)
                {
                    winT[i] = winT[i] + v.winT[i];
                    winOK[i] = winOK[i] + v.winOK[i];
                }
            }

            for (int i = 0; i < samplesT.Length; i++)
            {
                pUD[i] = (samplesT[i] / TotalSecT[i]) * 100;
                pUD[i] = Convert.ToDouble(pUD[i].ToString("0.###"));
            }
            for (int i = 0; i < winT.Length; i++)
            {
                pMD[i] = (winOK[i] / winT[i]) * 100;
                pMD[i] = Convert.ToDouble(pMD[i].ToString("0.###"));
            }


            /////UPDATE 2222222 and MLAT DET

            //////11111111
            //double[] minP = new double[] { 100, 100, 100 };
            //double[] num = new double[] { 0, 0, 0 };

            //foreach (Vehicle v in vl)
            //{
            //    v.Performance();
            //    for (int i = 0; i < v.pUD.Length; i++)
            //    {
            //        if (v.pUD[i] > 0)
            //        {
            //            num[i] = num[i] + 1;
            //            pUD[i] = pUD[i] + v.pUD[i];
            //            if (v.pUD[i] < minP[i])
            //            {
            //                minP[i] = v.pUD[i];
            //            }
            //        }
            //    }
            //}

            //for (int i = 0; i < pUD.Length; i++)
            //{
            //    pUD[i] = pUD[i] / num[i];
            //    pUD[i] = Convert.ToDouble(pUD[i].ToString("0.###"));
            //}
            ////1111111

            listPerf.Add(new ShowPerf("Maneuvering Area", pUD[0], 95.000, pMD[0], 99.9));
            listPerf.Add(new ShowPerf("Stand", pUD[1], 50.000, pMD[1], 99.9));
            listPerf.Add(new ShowPerf("Apron", pUD[2], 70.000, 0, 0));

        }

        private void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        private void UDInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It is the probability of achieving a 1 second average update rate for any Mode S transmitting target", "Probability Of Update");
        }

        private void MDInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It it the probability of detecting any Mode S transponder target" +
                " within a windows period set to 2 seconds for maneuvering area and 5 seconds for stands zone.\n"+
                "It is not defined for apron zone.\n", "Probability Of MLAT Detection");
        }
    }
}
