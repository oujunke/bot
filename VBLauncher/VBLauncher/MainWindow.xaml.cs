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
using System.Diagnostics;

namespace VBLauncher
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

        public static void StartGame()
        {
            Process.Start("Voice Bot.exe");
            Environment.Exit(0);
        }

        public static void StartWeb(String url)
        {
            Process.Start(url);
        }

        private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void LaunchWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            StartWeb("http://www.voicebot.gq");
        }
    }
}

