using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data; //for datatable
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Asterix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        List<Message> listMessages;
        DataTable dt; //taula que omple el grid

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                List<string> list = new List<string>();
                listMessages = new List<Message>();

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
                    i += length;
                }

                MessageBox.Show(listMessages.Count + " are loaded to the program.", "Loaded!",MessageBoxButton.OK, MessageBoxImage.Information);

                dt = new DataTable();
                dt.Columns.Add(new DataColumn("CAT", typeof(int)));
                dt.Columns.Add(new DataColumn("Length", typeof(int)));
                dt.Columns.Add(new DataColumn("FSPEC", typeof(string)));
                dt.Columns.Add(new DataColumn("Time", typeof(string)));
                dt.Columns.Add(new DataColumn("ICAO Address", typeof(string)));
                dt.Columns.Add(new DataColumn("CallSign", typeof(string)));
                dt.Columns.Add(new DataColumn("# Recievers", typeof(int)));

                foreach (Message m in listMessages)
                {
                    DataRow Row = dt.NewRow();
                    Row[0] = m.getCAT();
                    Row[1] = m.getLength();
                    Row[2] = m.getlistFSPEC();
                    Row[3] = m.getTOD().ToString("HH:mm:ss.ffff");
                    Row[4] = m.getAddressICAO().ToUpper();
                    Row[5] = m.getCallsign();
                    Row[6] = m.getTotalReceivers();
                    dt.Rows.Add(Row);
                    DataGrid.ItemsSource = dt.DefaultView;
                }
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
    }
}
