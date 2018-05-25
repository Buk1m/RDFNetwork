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

namespace RDFNetwork
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

        private void DragWindow( object sender, MouseButtonEventArgs e )
        {
            DragMove();
        }

        private void OnClickClose( object sender, RoutedEventArgs e )
        {
            Close();
        }

        private void OnClickMaximalize(object sender, RoutedEventArgs e)
        {
            WindowState ^= WindowState.Maximized;
        }

        private void OnClickMinimalize( object sender, RoutedEventArgs e )
        {
            WindowState = WindowState.Minimized;
        }

    }
}
