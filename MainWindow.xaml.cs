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
                StringBuilder sb = new StringBuilder();

                foreach (byte b in fileBytes)
                {
                    //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                    //TextBoxList.Text = 
                        MessageBox.Show(Convert.ToString(b,16).PadLeft(2, '0'));
                }
                MessageBox.Show("HOLA");
            }


        }
    }
}
