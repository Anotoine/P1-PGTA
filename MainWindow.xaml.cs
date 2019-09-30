using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data; //for datatable
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace P1_PGTA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        List<Message> listMessage;
        DataTable dt = new DataTable(); //taula qe omple el grid
        public MainWindow()
        {
            InitializeComponent();
            Point PARP = new Point(41.29, 2.09);
            Point P1 = new Point(41.309179, 2.045674);
            TextList.Text = "("+(100000*PARP.X).ToString() +";"+(100000*PARP.Y).ToString()+ ") - ("+(100000*P1.X).ToString() + ";" + (100000*P1.Y).ToString()+")";

            DataColumn CAT = new DataColumn("CAT", typeof(int));
            DataColumn Length = new DataColumn("Length", typeof(int));
            DataColumn FSPEC = new DataColumn("listFSPEC", typeof(string));
            dt.Columns.Add(CAT);
            dt.Columns.Add(Length);
            dt.Columns.Add(FSPEC);
            DataGrid.ItemsSource = dt.DefaultView;
        }

        private void Button_Click_File(object sender, RoutedEventArgs e)
        {
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

                foreach (Message m in listMessage)
                {
                    //TextList.Text = Convert.ToString(m.getCAT());
                    DataRow Row = dt.NewRow();
                    Row[0] = m.getCAT();
                    Row[1] = m.getLength();
                    Row[2] = m.getlistFSPEC();
                    dt.Rows.Add(Row);
                    DataGrid.ItemsSource = dt.DefaultView;
                }
            }
        }

        private void prova(object sender, MouseButtonEventArgs e)
        {
            TextList.Text = string.Join("", listMessage[DataGrid.SelectedIndex].getList().ToArray());
        }

        private void Button_Click_Map(object sender, RoutedEventArgs e)
        {
            Map map = new Map();
            map.Show();
            this.Close();
        }
    }
}
