using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace P1_PGTA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        List<Message> listMessage;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
                    int length = Int32.Parse(list[i+1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(list[i+2], System.Globalization.NumberStyles.HexNumber);
                    listMessage.Add(new Message(list.GetRange(i, length)));

                    i = i + length;
                }
                
                foreach (Message m in listMessage)
                {
                    TextList.Text = Convert.ToString(m.getCAT());
                    DataGrid.Items.Add(m);
                   
                }
            }
        }
    }
}
