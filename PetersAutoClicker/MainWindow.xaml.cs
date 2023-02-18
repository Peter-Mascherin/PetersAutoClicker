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

namespace PetersAutoClicker
{
   //We figured it out, we can now develop an autoclicker
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeLabelColour(object sender, RoutedEventArgs e)
        {
            thelabel.Foreground = Brushes.SteelBlue;
        }

        private void mainbtn_Click(object sender, RoutedEventArgs e)
        {
            anotherbtn.Foreground = Brushes.White;
            thelabel.Content = "Main Button clicked me";
            thelabel.Foreground = mainbtn.Background;
        }

        private void anotherbtn_Click(object sender, RoutedEventArgs e)
        {
            thelabel.Content = "Another Button clicked me";
            thelabel.Foreground = Brushes.Gold;
            mainbtn.Background = Brushes.RosyBrown;
        }
    }
}
