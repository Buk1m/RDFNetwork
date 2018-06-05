using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RDFNetwork.RBFApproximation.View;
using RDFNetwork.RBFApproximation.ViewModel;

namespace RDFNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = _mainViewModel;
            InitializeComponent();
        }

        private readonly MainViewModel _mainViewModel = new MainViewModel();

        private void Approximation_OnClick( object sender, RoutedEventArgs e )
        {
            ContentControl.DataContext = new RBFApproximation.ViewModel.ApproximationViewModel();
            ContentControl.Visibility = Visibility.Visible;
            _mainViewModel.TaskName = "Approximation RBF Network";
        }

        private void Classification_OnClick( object sender, RoutedEventArgs e )
        {
            ContentControl.DataContext = new RBFClassification.ViewModel.ClassificationViewModel();
            ContentControl.Visibility = Visibility.Visible;
            _mainViewModel.TaskName = "Classification RBF Network";
        }


        #region Sidebar Animations

        private bool animationCompleted;
        private bool mouseInside;

        private void DragWindow( object sender, MouseButtonEventArgs e )
        {
            DragMove();
        }

        private void OnClickClose( object sender, RoutedEventArgs e )
        {
            Close();
        }

        private void OnClickMaximalize( object sender, RoutedEventArgs e )
        {
            WindowState ^= WindowState.Maximized;
        }

        private void OnClickMinimalize( object sender, RoutedEventArgs e )
        {
            WindowState = WindowState.Minimized;
        }

        private async void method()
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay( 100 );
                if (!Sidebar.IsMouseOver)
                {
                    mouseInside = false;
                    return;
                }
            }

            if (mouseInside)
            {
                DoubleAnimation slideOut = new DoubleAnimation
                {
                    To = 180,
                    Duration = TimeSpan.FromSeconds( 0.6 ),
                    DecelerationRatio = 0.9
                };
                slideOut.Completed += ( s, ev ) => { animationCompleted = true; };
                slideOut.Completed += hideSidebar;
                await Dispatcher.BeginInvoke(
                    new Action( () => Sidebar.BeginAnimation( WidthProperty, slideOut ) ) );
            }
        }

        private async void Sidebar_OnMouseEnter( object sender, MouseEventArgs e )
        {
            mouseInside = true;
            await Dispatcher.BeginInvoke( new ThreadStart( () => method() ) );
        }


        private void Sidebar_OnMouseLeave( object sender, MouseEventArgs e )
        {
            mouseInside = false;
            if (animationCompleted)
            {
                DoubleAnimation slideOut = new DoubleAnimation
                {
                    To = 50,
                    Duration = TimeSpan.FromSeconds( 0.6 ),
                    DecelerationRatio = 0.9
                };

                Sidebar.BeginAnimation( WidthProperty, slideOut );
                animationCompleted = false;
            }
        }

        void hideSidebar( object sender, EventArgs e )
        {
            if (!mouseInside)
            {
                DoubleAnimation slideOut = new DoubleAnimation
                {
                    To = 50,
                    Duration = TimeSpan.FromSeconds( 1 ),
                    DecelerationRatio = 0.9
                };
                Sidebar.BeginAnimation( WidthProperty, slideOut );
                animationCompleted = false;
            }
        }

        #endregion

    }
}