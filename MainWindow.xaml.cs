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
        }

        private void BRadar_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Visible;
            WTable.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Hidden;
        }

        private void BTable_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Visible;
            WSettings.Visibility = Visibility.Hidden;
        }

        private void BSettings_Click(object sender, RoutedEventArgs e)
        {
            WLoad.Visibility = Visibility.Hidden;
            WRadar.Visibility = Visibility.Hidden;
            WTable.Visibility = Visibility.Hidden;
            WSettings.Visibility = Visibility.Visible;
        }
    }
}
