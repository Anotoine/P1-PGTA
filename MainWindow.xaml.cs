using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ASTERIX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        List<Message> listMessages;
        List<ShowRow> listRow;
        List<Tuple<string, string, string, string, string>> listPlaneDB;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_File(object sender, RoutedEventArgs e)
        {
            Load_db();

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                List<string> list = new List<string>();
                listMessages = new List<Message>();
                listRow = new List<ShowRow>();

                foreach (byte b in fileBytes)
                {
                    list.Add(Convert.ToString(b, 16).PadLeft(2, '0'));
                }

                int i = 0;
                while (i < list.Count)
                {
                    int length = Int32.Parse(list[i + 1] + list[i + 2], System.Globalization.NumberStyles.HexNumber);
                    Message m = new Message(list.GetRange(i, length));

                    listMessages.Add(m);
                    listRow.Add(new ShowRow(m,listPlaneDB));
                    i += length;
                }

                MessageBox.Show(listMessages.Count + " are loaded to the program.", "Loaded!",MessageBoxButton.OK, MessageBoxImage.Information);

                DataGrid.ItemsSource = listRow;
            }
        }

        private void MouseClickTable(object sender, MouseButtonEventArgs e)
        {
            //TextList.Text = string.Join("", listMessages[DataGrid.SelectedIndex].getList().ToArray());
        }

        private void Button_Click_Map(object sender, RoutedEventArgs e)
        {
            if (listMessages == null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to open the map editor before loading any data?", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Map map = new Map(listMessages);
                    map.Show();
                    this.Close();
                }
            } else {
                Map map = new Map(listMessages);
                map.Show();
                this.Close();
            }
        }
        private void Load_db()
        {
            string path = Directory.GetCurrentDirectory();
            if (File.Exists(path + @"\_data\aircraft_db.csv"))
            {
                listPlaneDB = new List<Tuple<string, string, string, string, string>>();
                using (var reader = new StreamReader(path + @"\_data\aircraft_db.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        var val = reader.ReadLine().Split(',');
                        listPlaneDB.Add(new Tuple<string, string, string, string, string>(val[0], val[1], val[2], val[3], val[4]));
                    }
                }
            }
            else
            {
                MessageBox.Show("No aircraft_db.csv file found in " + path + @"\_data\aircraft_db.csv.", "Warning");
            }
        }
    }
}
