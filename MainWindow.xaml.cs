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

        List<Message> listMessage;
        DataTable dt; //taula qe omple el grid
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_File(object sender, RoutedEventArgs e)
        {
            DataColumn CAT = new DataColumn("CAT", typeof(int));
            DataColumn Length = new DataColumn("Length", typeof(int));
            DataColumn FSPEC = new DataColumn("listFSPEC", typeof(string));
            DataColumn ToD = new DataColumn("ToD", typeof(string));
            dt = new DataTable();
            dt.Columns.Add(CAT);
            dt.Columns.Add(Length);
            dt.Columns.Add(FSPEC);
            dt.Columns.Add(ToD);
            DataGrid.ItemsSource = dt.DefaultView;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                List<string> list = new List<string>();
                listMessage = new List<Message>();

                foreach (byte b in fileBytes)
                {
                    list.Add(Convert.ToString(b, 16).PadLeft(2, '0'));
                }

                int i = 0;
                while (i < list.Count)
                {
                    int length = Int32.Parse(list[i + 1] + list[i + 2], System.Globalization.NumberStyles.HexNumber);
                    Message m = new Message(list.GetRange(i, length));
                    listMessage.Add(m);
                    i += length;
                }

                MessageBox.Show(listMessage.Count + " are loaded to the program.", "Loaded!",MessageBoxButton.OK, MessageBoxImage.Information);

                foreach (Message m in listMessage)
                {
                    DataRow Row = dt.NewRow();
                    Row[0] = m.getCAT();
                    Row[1] = m.getLength();
                    Row[2] = m.getlistFSPEC();
                    Row[3] = m.getTOD();
                    dt.Rows.Add(Row);
                    DataGrid.ItemsSource = dt.DefaultView;
                }
            }
        }

        private void MouseClickTable(object sender, MouseButtonEventArgs e)
        {
            //TextList.Text = string.Join("", listMessage[DataGrid.SelectedIndex].getList().ToArray());
        }

        private void Button_Click_Map(object sender, RoutedEventArgs e)
        {
            Map map = new Map();
            map.Show();
            this.Close();
        }
    }
}
