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
    public partial class MainWindow : Window
    {
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
                StringBuilder sb = new StringBuilder();

                foreach (byte b in fileBytes)
                {
                    list.Add(Convert.ToString(b, 16).PadLeft(2, '0'));
                }
                TextList.Text = list.ToString();

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == "15" && list[i + 1] == "00" && list[i + 2] == "28")
                    {
                        for (int j = i+1; j < list.Count; j++)
                        {
                            if (list[j] == "15" && list[j + 1] == "00" && list[j + 2] == "28")
                            {
                                foreach (string line in list.GetRange(i,j-i))
                                {
                                    sb.Append(line);
                                }
                                sb.AppendLine();
                            }
                        }
                    }
                }

                TextList.Text = sb.ToString();
            }


        }
    }
}
